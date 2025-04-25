//#define DEBUG_GC
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Media;
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
        private ModAsset asset;
        private bool marked;

        private VidPlayerEntry(Video vid, VideoPlayer vidPlayer, ModAsset modAsset) {
            video = vid;
            videoPlayer = vidPlayer;
            asset = modAsset;
        }
        
        public static VidPlayerEntry Create(ModAsset videoTargetAsset) {
            string cachePath = videoTargetAsset.GetCachedPath();
            Video video = Engine.Instance.Content.Load<Video>(cachePath);
            VideoPlayer videoPlayer = new();
            return new VidPlayerEntry(video, videoPlayer, videoTargetAsset);
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
}
