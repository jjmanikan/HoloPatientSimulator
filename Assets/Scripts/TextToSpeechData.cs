using System;
using System.Collections.Generic;

namespace TTSJsonData
{
    [Serializable]
    public class TTSRequestBody
    {
        public Input input;
        public Voice voice;
        public AudioConfig audioConfig;
    }

    [Serializable]
    public class Input
    {
        public String input;
    }

    [Serializable]
    public class Voice
    {
        public String languageCode;
        public String name;
        public SSMLGender ssmlGender;
    }

    [Serializable]
    public class AudioConfig
    {
        public AudioEncoding audioEncoding;
    }

    [Serializable]
    public enum SSMLGender
    {
        MALE,
        FEMALE
    }

    [Serializable]
    public enum AudioEncoding
    {
        MP3
    }
    
    [Serializable]
    public class TTSResponseBody
    {
        public String audioContent; 
    }

}