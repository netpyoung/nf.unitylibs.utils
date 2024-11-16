using System;
using System.Runtime.CompilerServices;

namespace NF.UnityLibs.Utils
{
    public static class MathUtil
    {
        /// <summary>
        /// Round_Normal 은 일반적인 반올림 (Round Half Up).
        ///
        /// .NET의 Math.Round 메서드는 기본적으로 "Banker's Rounding" 방식을 사용. 가장 가까운 짝수로 반올림. 실수하기 쉬움.
        /// 
        /// ```cs
        /// Math.Round(2.5, MidpointRounding.AwayFromZero); // 결과: 3 | Round Half Up
        /// Math.Round(2.5);                                // 결과: 2 | Round Half to Even
        /// ```
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round_Normal(float v)
        {
            return MathF.Round(v, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// NOTE(pyoung): 진짜 이 함수가 필요시 private제거하여 살림.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Round_Banker(float v)
        {
            return MathF.Round(v);
        }
    }
}