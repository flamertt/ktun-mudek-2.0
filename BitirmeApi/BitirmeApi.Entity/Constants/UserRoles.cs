namespace BitirmeApi.Entity.Constants
{
    /// <summary>
    /// Merkezi rol sabitleri — büyük/küçük harf tutarsızlığını önler.
    /// Tüm servis ve kontrolcüler bu sabitleri kullanmalı.
    /// </summary>
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";
    }
}
