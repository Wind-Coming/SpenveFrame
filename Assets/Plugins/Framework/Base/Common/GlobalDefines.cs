using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spenve
{
    public enum NumMode
    {
        Constant,
        RandomRange,
        RandomTwoConstant
    }

    [Serializable]
    public struct NumStruct
    {
        public NumMode mode;
        [ShowIf("mode", NumMode.Constant)]
        public float num;
        [ShowIf("mode", NumMode.RandomRange)]
        public Vector2 range;
        [ShowIf("mode", NumMode.RandomTwoConstant)]
        public Vector2 two;

        public float Get()
        {
            if (mode == NumMode.Constant)
            {
                return num;
            }
            else if(mode == NumMode.RandomRange)
            {
                return Random.Range(range.x, range.y);
            }
            else
            {
                return Random.value < 0.5f ? two.x : two.y;
            }
        }
    }
}
