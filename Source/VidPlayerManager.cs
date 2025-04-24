using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Monocle;

namespace Celeste.Mod.VidPlayer;

public static class VidPlayerManager {
    private static readonly Dictionary<ModAsset, WeakReference<VidPlayerEntry>> players = new();

    public static VidPlayerEntry GetPlayerFor(string id) {
        if (!Everest.Content.TryGet(id, out ModAsset videoTargetAsset)) {
            throw new FileNotFoundException("Could not find video target: " + id);
        }
        if (players.TryGetValue(videoTargetAsset, out WeakReference<VidPlayerEntry>? weakRef) && weakRef.TryGetTarget(out VidPlayerEntry? entry) && !entry.videoPlayer.IsDisposed) return entry;
        entry = VidPlayerEntry.Create(videoTargetAsset);
        if (weakRef != null) {
            weakRef.SetTarget(entry);
        } else {
            weakRef = new WeakReference<VidPlayerEntry>(entry);
        }
        players[videoTargetAsset] = weakRef;
        return entry;
    }

    public record VidPlayerEntry : IDisposable {
        public readonly VideoPlayer videoPlayer;
        public readonly Video video;

        private VidPlayerEntry(Video vid, VideoPlayer vidPlayer) {
            video = vid;
            videoPlayer = vidPlayer;
        }
        
        public static VidPlayerEntry Create(ModAsset videoTargetAsset) {
            string cachePath = videoTargetAsset.GetCachedPath();
            Video video = Engine.Instance.Content.Load<Video>(cachePath);
            VideoPlayer videoPlayer = new();
            return new VidPlayerEntry(video, videoPlayer);
        }
        
        public void Dispose() {
            videoPlayer.Dispose();
        }
        
        ~VidPlayerEntry() {
            videoPlayer.Dispose();
#if DEBUG_GC
            Console.WriteLine("VidPlayerManager died");
#endif
        }
    }
}