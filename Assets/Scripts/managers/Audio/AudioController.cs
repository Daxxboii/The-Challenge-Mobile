﻿
using System;
using System.Collections;
using UnityEngine;

namespace unityCore
{
    namespace Audio
    {
        public class AudioController : MonoBehaviour
        {
            //members
            public static AudioController instance;
            public bool debug;
            public AudioTrack[] tracks;

            private Hashtable m_audioTable;
            private Hashtable m_jobTable;



            [System.Serializable]
            public class AudioObject 
            { 
                public AudioType type;
                public AudioClip Clip;
            }




            [System.Serializable]
            public class AudioTrack 
            {
                public AudioSource source;
                public AudioObject[] audio;
            }


            private class AudioJob 
            {
                public AudioAction action;
                public AudioType type;

                public AudioJob(AudioAction _action, AudioType _type)
                {
                    action = _action;
                    type = _type;
                }
            }

            private enum AudioAction
            {
                START,
                STOP,
                RESTART
            }




            #region Unity Functions
            private void Awake()
            {
                // instance
                if(!instance)
                {
                    Configure();
                    //configure

                }
            }
            private void OnDisable()
            {
                Dispose();
            }
            #endregion



            #region Public Functions

            public void PlayAudio(AudioType _type)
            {
                AddJob(new AudioJob(AudioAction.START , _type));
            }

            public void StopAudio(AudioType _type)
            {
                AddJob(new AudioJob(AudioAction.STOP, _type));
            }

            public void RestartAudio(AudioType _type)
            {
                AddJob(new AudioJob(AudioAction.RESTART, _type));
            }

            #endregion



            #region private Functions
            private void log(string _msg)
            {
                if (!debug) return;
                Debug.Log("[AudioController]: " + _msg);
            }
            private void logWarning(string _msg)
            {
                if (!debug) return;
                Debug.LogWarning("[AudioController]: " + _msg);
            }
            private void Configure()
            {
                instance = this;
                m_audioTable = new Hashtable();
                m_jobTable = new Hashtable();
                GenerateAudio();
            }
            private void Dispose()
            {
                
            }
            private void GenerateAudio()
            {
                foreach (AudioTrack _tracks in tracks)
                {
                    foreach (AudioObject _obj in _tracks.audio)
                    {
                        if (m_audioTable.ContainsKey(_obj.type))
                        {
                            m_audioTable.Add(_obj.type, _tracks);
                            log("registreing Audio lamo : " + _obj.type);
                        }
                    }
                }
            }
            private void AddJob(AudioJob _job)
            {
                //Remove job which are coexisting and can make skertie and manish mad
                RemoveConflictingJobs(_job.type);
                //Add jobs
                IEnumerator JobRunner = RunAudioJob(_job);
                m_jobTable.Add(_job.type, JobRunner);
                log("Start job on" + _job.type + "with operation" + _job.action);
            }
            private void RemoveConflictingJobs(AudioType _type)
            {
                if(m_jobTable.ContainsKey(_type))
                {
                    RemoveJob(_type);
                }
                AudioType _conflictAudio = AudioType.None;
                foreach(DictionaryEntry _entry in m_jobTable)
                {
                    AudioType _audioType = (AudioType)_entry.Key;
                    AudioTrack _audioTrackInUse = (AudioTrack)m_audioTable[_audioType];
                    AudioTrack _audioTrackNeeded = (AudioTrack)m_audioTable[_type];
                    if (_audioTrackNeeded.source == _audioTrackInUse.source)
                    {
                        //conflict
                        _conflictAudio = _audioType;
                    }

                }
            }
            #endregion

        }
    }
}