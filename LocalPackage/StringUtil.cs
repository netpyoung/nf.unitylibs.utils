namespace NF.UnityLibs.Utils
{
    public static class StringUtil
    {
        // 예를 들어 Split 함수나 문자열 비교 함수들
        // ignorecase - OrdinalIgnoreCase  invalidculture
        // https://learn.microsoft.com/ko-kr/dotnet/api/system.stringcomparison?view=net-8.0#system-stringcomparison-invariantculture
        // StringComparison.CurrentCulture
        // 대소문자를 구분합니다 (case-sensitive).
        // 현재 스레드의 문화권 설정을 사용하여 문자열을 비교합니다.

        // 영어(en-US)와 독일어(de-DE) 문화권에서는 'ä'가 'a' 뒤에 정렬됩니다.
        // 하지만 스웨덴어(sv-SE) 문화권에서는 'ä'가 'a' 앞에 정렬됩니다.
        // | en-US | string.Compare("ä", "a", StringComparison.CurrentCulture) | 1
        // | de-DE | string.Compare("ä", "a", StringComparison.CurrentCulture) | 1
        // | sv-SE | string.Compare("ä", "a", StringComparison.CurrentCulture) | -1
    }
}