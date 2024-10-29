using System;
using System.Runtime.CompilerServices;

namespace NF.UnityLibs.Utils
{
    public static class MathUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Round_Normal(float v)
        {
            // NOTE(pyoung):
            // .NET의 Math.Round 메서드는 기본적으로 "Banker's Rounding" 방식을 사용. 가장 가까운 짝수로 반올림.
            // Math.Round(2.5); // 결과: 2
            // Math.Round(2.5, MidpointRounding.AwayFromZero); // 결과: 3

            return MathF.Round(v, MidpointRounding.AwayFromZero);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float Round_Banker(float v)
        {
            // NOTE(pyoung): 진짜 이 함수가 필요시 private제거하여 살림.
            return MathF.Round(v);
        }
    }
}