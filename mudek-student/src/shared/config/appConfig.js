export const appConfig = {
  role: 'student',
  appTitle: 'MUDEK Öğrenci Paneli',
  sidebarTitle: 'Öğrenci Paneli',
  storage: {
    tokenKey: 'mudek_student_token',
    userKey: 'mudek_student_user',
    themeKey: 'mudek_student_theme',
  },
  routes: {
    home: '/home',
    courses: '/courses',
    surveys: '/surveys',
  },
  navSections: [
    {
      key: 'main',
      title: '',
      items: [
        { key: 'home', label: 'Anasayfa', path: '/home', icon: 'home' },
        { key: 'courses', label: 'Derslerim', path: '/courses', icon: 'book-open' },
        { key: 'surveys', label: 'Anketlerim', path: '/surveys', icon: 'clipboard-list' },
      ],
    },
  ],
  home: {
    title: 'Öğrenci Anasayfa',
    subtitle: 'Ders kayıtları, notlar ve MUDEK çıktıları için özet ekran.',
    stats: [
      { label: 'Kayıtlı Ders', value: '7' },
      { label: 'Değerlendirme', value: '12' },
      { label: 'Bekleyen Sınav', value: '3' },
      { label: 'CLO Tamamlanma', value: '%76' },
    ],
    highlights: [
      'my-courses/{offeringId}/students',
      'evaluations/{evaluationId}/exams',
      'exam-questions/{questionId}/answers',
      'mudek-evaluation/results',
    ],
  },
  pages: {
    courses: {
      title: 'Derslerim',
      description: 'Kayıtlı dersler, sınavlar ve değerlendirme detayları burada listelenecek.',
    },
    surveys: {
      title: 'Anketlerim',
      description: 'Aktif ve tamamlanan anket listesi bu ekranda gösterilecek.',
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
    searchPlaceholder: 'Ders, sınav veya anket ara…',
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

