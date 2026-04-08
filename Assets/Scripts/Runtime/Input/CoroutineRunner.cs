using System.Collections;
using UnityEngine;
using Yujanggi.Core.Domain;

namespace Yujanggi.Runtime.Input
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {
        public Coroutine Run(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }
        public void Stop(Coroutine routine)
        {
            StopCoroutine(routine);
        }
    }

}