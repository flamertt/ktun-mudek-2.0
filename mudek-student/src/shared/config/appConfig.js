export const appConfig = {
  role: 'student',
  appTitle: 'MUDEK Öğrenci Anketleri',
  sidebarTitle: 'Anketler',
  storage: {
    tokenKey: 'mudek_student_token',
    userKey: 'mudek_student_user',
    themeKey: 'mudek_student_theme',
  },
  routes: {
    surveys: '/surveys',
  },
  navSections: [
    {
      key: 'main',
      title: '',
      items: [{ key: 'surveys', label: 'Anketlerim', path: '/surveys', icon: 'clipboard-list' }],
    },
  ],
  pages: {
    surveys: {
      title: 'Anketlerim',
      description: 'Aktif anketi olan dersleri listeler; ders seçerek anketleri doldurabilirsiniz.',
    },
  },
  ui: {
    logoAlt: 'Kurum logosu',
    universityLogoAlt: 'KTUN logosu',
    collapseOpen: 'Aç',
    collapseClose: 'Kapat',
    themeDark: 'Dark',
    themeLight: 'Light',
    toggleSidebarLabel: 'Sidebar aç kapa',
    toggleThemeLabel: 'Tema değiştir',
    toggleFullscreenLabel: 'Tam ekran',
    messagesLabel: 'Mesajlar',
    notificationsLabel: 'Bildirimler',
    settingsLabel: 'Ayarlar',
    searchInputLabel: 'Arama',
    searchPlaceholder: 'Ders veya anket ara…',
    breadcrumbAriaLabel: 'Sayfa konumu',
    breadcrumbRoot: 'MUDEK',
    profileLabel: 'Profil',
    profileMenuAriaLabel: 'Hesap menüsü',
    profileName: 'Öğrenci Kullanıcı',
    profileRole: 'Öğrenci',
    logoutLabel: 'Güvenli Çıkış',
    logoutError: 'Çıkış yapılırken bir hata oluştu.',
    homeFocusTitle: 'API tabanlı mock odak alanları',
  },
}

