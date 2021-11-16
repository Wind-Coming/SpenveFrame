using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Spenve
{
    public class TimerSystem : SingletonAutoCreate<TimerSystem>
    {
        List<Timer> sts = new List<Timer>();

        public Timer Add(Action _func, float _time, bool _loop = false)
        {
            Timer st = ClassPool<Timer>.Get();
            st.Init(_func, _time, _loop);
            st.Start();
            return st;
        }

        public void Push(Timer std)
        {
            sts.Add(std);
        }

        public void Pop(Timer std)
        {
            if(!std.active)
                return;
            
            sts.Remove(std);
            ClassPool<Timer>.Release(std);
        }

        public void Clear()
        {
            for (int i = 0; i < sts.Count; i++)
            {
                Timer st = sts[i];
                st.Stop();
            }
        }

        void Update()
        {
            for (int i = 0; i < sts.Count; i++)
            {
                Timer st = sts[i];
                st.curTime += Time.deltaTime;

                if(st.update != null)
                {
                    st.update();
                }

                if (st.curTime >= st.time)
                {
                    st.func();
                    if (st.loop)
                    {
                        st.curTime -= st.time;
                    }
                    else
                    {
                        st.Stop();
                        i--;
                        continue;
                    }
                }
            }
        }
    }

    public class Timer: IReset
    {
        public Action func;
        public bool loop;
        public float time;
        public float curTime;
        public Action update;

        public bool active = false;

        public Timer()
        {
        }
        
        public void Init(Action _func, float _time, bool _loop = false)
        {
            active = true;
            func = _func;
            time = _time;
            loop = _loop;
        }

        public void Start()
        {
            curTime = 0;
            TimerSystem.Instance.Push(this);
        }

        public void Stop()
        {
            TimerSystem.Instance.Pop(this);
        }

        public void Reset()
        {
            active = false;
        }
    }
}