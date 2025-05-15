//#define DEBUG_GC
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Media;
using VideoPlayer = Celeste.Mod.VidPlayer.FNA_Reimpl.VideoPlayer2;
using Monocle;

namespace Celeste.Mod.VidPlayer;

public static class VidPlayerManager {
    private static readonly Dictionary<ModAsset, WeakReference<VidPlayerEntry>> players = new();
    private static readonly HashSet<VidPlayerEntry> collectionList = new();

    public static VidPlayerEntry GetPlayerFor(string id) {
        if (!Everest.Content.TryGet(id, out ModAsset videoTargetAsset)) {
            throw new FileNotFoundException("Could not find video target: " + id);
        }
        if (players.TryGetValue(videoTargetAsset, out WeakReference<VidPlayerEntry>? weakRef) && weakRef.TryGetTarget(out VidPlayerEntry? entry) && !entry.videoPlayer.IsDisposed) {
            return entry;
        }
        entry = VidPlayerEntry.Create(videoTargetAsset);
        if (weakRef != null) {
            weakRef.SetTarget(entry);
        } else {
            weakRef = new WeakReference<VidPlayerEntry>(entry);
        }
        players[videoTargetAsset] = weakRef;
        return entry;
    }

    public static void Collect() {
        if (collectionList.Count == 0) return;
        foreach (VidPlayerEntry entry in collectionList) {
            entry.videoPlayer.Stop();
            entry.Dispose();
        }
        collectionList.Clear();
    }

    public record VidPlayerEntry : IDisposable {
        public readonly VideoPlayer videoPlayer;
        public readonly Video video;
        public readonly ModAsset asset;
        public readonly string usedHandle;
        private bool marked;
        internal bool Marked => marked;

        private VidPlayerEntry(Video vid, VideoPlayer vidPlayer, ModAsset modAsset, string usedPath) {
            video = vid;
            videoPlayer = vidPlayer;
            asset = modAsset;
            usedHandle = usedPath;
        }
        
        public static VidPlayerEntry Create(ModAsset videoTargetAsset) {
            string cachePath = videoTargetAsset.GetCachedPath();
            Video video = Engine.Instance.Content.Load<Video>(cachePath);
            VideoPlayer videoPlayer = new();
            return new VidPlayerEntry(video, videoPlayer, videoTargetAsset, cachePath);
        }

        public void MarkForCollection() {
            if (marked) return;
            marked = true;
            collectionList.Add(this);
        }

        public void SaveFromCollection() {
            collectionList.Remove(this);
            marked = false;
        }
        
        public void Dispose() {
            videoPlayer.Dispose();
            GC.SuppressFinalize(this);
        }

        // Entities that aren't on savestates may orphan themselves without disposing properly
        // Shouldn't happen anymore
        ~VidPlayerEntry() {
            videoPlayer.Dispose();
            
#if DEBUG_GC
            Console.WriteLine("VidPlayerManager died");
#endif
        }
    }
    
    // Commands
    [Command("vp_alive_players", "Prints to console the amount and which video player instances are currently alive.")]
    private static void GetAliveVideoPlayers() {
        Engine.Commands.Log($"Currently alive players:");
        int count = 0;
        foreach ((ModAsset key, WeakReference<VidPlayerEntry> value) in players) {
            if (value.TryGetTarget(out VidPlayerEntry? videoPlayer)) {
                count++;
                LogVideoPlayer(videoPlayer);
            }
        }
        Engine.Commands.Log($"Total: {count}");
    }

    [Command("vp_collecting_players", "Gets the video players marked for collection.")]
    private static void CollectingVideoPlayers() {
        Engine.Commands.Log($"Collectable video players:");
        foreach (VidPlayerEntry vidEntry in collectionList) {
            LogVideoPlayer(vidEntry);
        }
        Engine.Commands.Log($"Total: {collectionList.Count}");
    }

    [Command("vp_force_gc", "Forces a gc collect call.")]
    private static void ForceGc(bool aggresive = false) {
        GC.Collect(GC.MaxGeneration, aggresive ? GCCollectionMode.Aggressive : GCCollectionMode.Forced, true, aggresive);
    }

    private static void LogVideoPlayer(VidPlayerEntry videoPlayer) {
        Engine.Commands.Log($"- {videoPlayer.asset.PathVirtual}");
        Engine.Commands.Log($"  IsDisposed: {videoPlayer.videoPlayer.IsDisposed}");
        Engine.Commands.Log($"  IsLooped: {videoPlayer.videoPlayer.IsLooped}");
        Engine.Commands.Log($"  IsMuted: {videoPlayer.videoPlayer.IsMuted}");
        Engine.Commands.Log($"  State: {videoPlayer.videoPlayer.State}");
        Engine.Commands.Log($"  Volume: {videoPlayer.videoPlayer.Volume}");
        Engine.Commands.Log($"  PlayPosition: {videoPlayer.videoPlayer.PlayPosition}");
        Engine.Commands.Log($"  Marked: {videoPlayer.Marked}");
    }
}
