using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Media;
using Monocle;

namespace Celeste.Mod.VidPlayer;

public class VidPlayerManager {
    public static VidPlayerManager Instance { get; } = new();

    private static readonly Dictionary<string, WeakReference<VidPlayerEntry>> players = new();

    public static VidPlayerEntry GetPlayerFor(string id) {
        if (players.TryGetValue(id, out WeakReference<VidPlayerEntry>? weakRef) && weakRef.TryGetTarget(out VidPlayerEntry? entry)) return entry;
        entry = VidPlayerEntry.Create(id);
        if (weakRef != null) {
            weakRef.SetTarget(entry);
        } else {
            weakRef = new WeakReference<VidPlayerEntry>(entry);
        }
        players[id] = weakRef;
        return entry;
    }

    public record VidPlayerEntry : IDisposable {
        public readonly VideoPlayer videoPlayer;
        public readonly Video video;

        private VidPlayerEntry(Video vid, VideoPlayer vidPlayer) {
            video = vid;
            videoPlayer = vidPlayer;
        }
        
        public static VidPlayerEntry Create(string name) {
            if (!Everest.Content.TryGet(name, out ModAsset videoTargetAsset)) {
                throw new FileNotFoundException("Could not find video target: " + name);
            }
            
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