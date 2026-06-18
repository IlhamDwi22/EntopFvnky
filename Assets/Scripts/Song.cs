    using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Song
{
    public bool ready;
    public FileInfo fileInfo;
    public Audio audio;
    public Data data;

    [System.Serializable]
    public class Audio
    {
        public AudioClip guitar;
        public AudioClip song;
        public AudioClip rhythm;
    }

    [System.Serializable]
    public class Data
    {
        public Info info;
        public List<SyncTrack> syncTrack;
        public List<SongEvent> events;
        public Notes notes;
    }

    [System.Serializable]
    public class Info
    {
        public uint resolution = 192;
    }

    [System.Serializable]
    public class SyncTrack
    {
        public uint timestamp;
        public string type;
        public uint value;

        public SyncTrack(uint timestamp, string type, uint value)
        {
            this.timestamp = timestamp;
            this.type = type;
            this.value = value;
        }
    }

    [System.Serializable]
    public class SongEvent
    {
        public uint timestamp;
        public string name;

        public SongEvent(uint timestamp, string name)
        {
            this.timestamp = timestamp;
            this.name = name;
        }
    }

    [System.Serializable]
    public class Notes
    {
        public List<Note> easy;
        public List<Note> medium;
        public List<Note> hard;
        public List<Note> expert;
    }

    [System.Serializable]
    public class Note
    {
        public uint timestamp;
        public uint fred;          // Jalur / Lane (0-4)
        public uint length;        // Durasi hold note (sustain)
        public bool star;          // Data star power
        public bool hammerOn;

        public Note(uint timestamp, uint fred, uint length, bool star, bool hammerOn)
        {
            this.timestamp = timestamp;
            this.fred = fred;
            this.length = length;
            this.star = star;
            this.hammerOn = hammerOn;
        }
    }
}