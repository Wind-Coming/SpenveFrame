using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Spenve
{
    public class MsgSystem : SingleInstance<MsgSystem>
    {

        interface IActionType
        {

        }
        class ActionType<T> : IActionType
        {
            public Action<T> receives = obj => { };
        }

        class CallLua
        {
            public Action<object, object> func;
            public object tb;
        }

        Dictionary<string, Action> msgArg0Event = new Dictionary<string, Action>();
        Dictionary<string, IActionType> msgArg1Event = new Dictionary<string, IActionType>();
        
        Dictionary<string, List<CallLua>> msgLuaEvent = new Dictionary<string, List<CallLua>>();
        
      
        /// <summary>
        /// 添加方法以及函数
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="func"></param>
        public void AddListener(string methodName, Action func )
        {
            if (!msgArg0Event.ContainsKey(methodName))
            {
                msgArg0Event.Add(methodName, new Action( () => { }));
            }

            if (!msgArg0Event[methodName].GetInvocationList().Contains(func))
            {
                msgArg0Event[methodName] += func;
            }
        }

        public void AddListener(string methodName, Action<object, object> func, object tb)
        {
            if (!msgLuaEvent.TryGetValue(methodName, out var list))
            {
                list = new List<CallLua>();
                msgLuaEvent[methodName] = list;
            }

            foreach (var callLua in list)
            {
                if (callLua.tb == tb)
                {
                    callLua.func += func;
                    return;
                }
            }
            
            list.Add(new CallLua
            {
                func = func,
                tb = tb,
            });
        }
        
        public void AddListener<T>(string methodName, Action<T> func)
        {
            if (!msgArg1Event.ContainsKey(methodName))
            {
                var reg = new ActionType<T>();
                msgArg1Event.Add(methodName, reg);
            }

            var rega = msgArg1Event[methodName] as ActionType<T>;

            if (!rega.receives.GetInvocationList().Contains(func))
            {
                rega.receives += func;
            }
        }

        /// <summary>
        /// 移除函数
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="func"></param>
        public void RemoveListener(string methodName, Action func)
        {
            if (msgArg0Event.ContainsKey(methodName))
            {
                if (msgArg0Event[methodName].GetInvocationList().Contains(func))
                {
                    msgArg0Event[methodName] -= func;
                }
            }
        }

        public void RemoveListener(string methodName, Action<object, object> func, object tb)
        {
            if (!msgLuaEvent.TryGetValue(methodName, out var list))
            {
                return;
            }

            foreach (var callLua in list.Where(callLua => callLua.tb == tb))
            {
                callLua.func -= func;
                return;
            }
        }

        public void RemoveListener<T>(string methodName, Action<T> func)
        {
            if (msgArg1Event.ContainsKey(methodName))
            {
                var rega = msgArg1Event[methodName] as ActionType<T>;

                if ( rega.receives.GetInvocationList().Contains(func))
                {
                    rega.receives -= func;
                }
            }
        }

        /// <summary>
        /// 移除方法
        /// </summary>
        /// <param name="methodName"></param>
        public void RemoveMethod(string methodName)
        {
            msgArg0Event.Remove(methodName);
            msgArg1Event.Remove(methodName);
        }


        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="methodName"></param>
        public void PostMessage(string methodName)
        {
            if (msgArg0Event.ContainsKey(methodName))
            {
                msgArg0Event[methodName]();
            }

            if (!msgLuaEvent.TryGetValue(methodName, out var list)) return;
            foreach (var callLua in list)
            {
                callLua.func?.Invoke(callLua.tb, null);
            }
        }

        public void PostMessage<T>(string methodName, T obj)
        {
            if (msgArg1Event.ContainsKey(methodName))
            {
                var rega = msgArg1Event[methodName] as ActionType<T>;
                rega.receives(obj);
            }

            if (!msgLuaEvent.TryGetValue(methodName, out var list)) return;
            foreach (var callLua in list)
            {
                callLua.func?.Invoke(callLua.tb, obj);
            }
        }

    }
    public delegate void UIEvent(params object[] msg);
    public class EventDispitcher:SingleInstance<EventDispitcher>
    {

        Dictionary<object, Dictionary<string, UIEvent>> events = new Dictionary<object, Dictionary<string, UIEvent>>();
        public void EventsOn(object v, string name , UIEvent e  )
        {
            if (v == null)
                v = EventDispitcher.Instance;

            Dictionary<string, UIEvent> a = null;
            if(events.TryGetValue(v , out a))
            {


                UIEvent a1 = null;
                if(a.TryGetValue(name,out a1 ))
                {
                    a1 += e;
                    return;
                }
                else
                {
                    a[name] = e;
                    return;
                }
            }
            a = new Dictionary<string, UIEvent>();
            a[name] = e;
            events[v] = a;
        }

        public void Remove(object v ,string name, UIEvent e)
        {
            if (v == null)
                v = EventDispitcher.Instance;

            Dictionary<string, UIEvent> a = null;
            if (events.TryGetValue(v, out a))
            {
                UIEvent a1 = null;
                if (a.TryGetValue(name, out a1))
                {
                    a1 -= e;
                    return;
                }
            }
        }

        public void Remove(object v ,string  name )
        {
            if (v == null)
                v = EventDispitcher.Instance;
            Dictionary<string, UIEvent> a = null;
            if (events.TryGetValue(v, out a))
            {
                UIEvent a1 = null;
                if (a.TryGetValue(name, out a1))
                {
                    a.Remove(name);
                }
            }
        }

        public void Remove (object v)
        {
            Dictionary<string, UIEvent> a = null;
            if (events.TryGetValue(v, out a))
            {
                events.Remove(v);
            }
        }

        public void Emit(string name, params object[] msg)
        {
            foreach (var  kv in events)
            {
                foreach (var kv2 in kv.Value.Keys)
                {
                    if(kv2 == name)
                    {
                        kv.Value[name].Invoke(msg);
                    }
                }
            }
        }

        public void Emit(object v,string name ,params object[] msg)
        {
            Dictionary<string, UIEvent> a = null;
            if (events.TryGetValue(v, out a))
            {
                UIEvent a2 = null;
                if(a.TryGetValue(name ,out a2))
                {
                    a2.Invoke(msg);
                }
            }
        }
    }
}
