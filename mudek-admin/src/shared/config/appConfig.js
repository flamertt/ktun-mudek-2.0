export const appConfig = {
  role: 'admin',
  appTitle: 'MUDEK Admin Paneli',
  sidebarTitle: 'Admin',
  storage: {
    tokenKey: 'mudek_admin_token',
    userKey: 'mudek_admin_user',
    themeKey: 'mudek_admin_theme',
  },
  routes: {
    home: '/home',
    userManagement: '/user-management',
    teacherManagement: '/teacher-management',
    facultyManagement: '/faculty-management',
    programOutcomes: '/program-outcomes',
    courseManagement: '/course-management',
    studentManagement: '/student-management',
    courseStudents: '/course-students',
    surveyManagement: '/survey-management',
  },
  navSections: [
    {
      key: 'home',
      title: '',
      items: [{ key: 'home', label: 'Anasayfa', path: '/home', icon: 'home' }],
    },
    {
      key: 'users',
      title: 'Kullanıcı & Personel',
      items: [
        {
          key: 'userManagement',
          label: 'Kullanıcı Yönetimi',
          path: '/user-management',
          icon: 'users',
        },
        {
          key: 'teacherManagement',
          label: 'Öğretmen Yönetimi',
          path: '/teacher-management',
          icon: 'graduation-cap',
        },
      ],
    },
    {
      key: 'academic',
      title: 'Akademik Yapı',
      items: [
        {
          key: 'facultyManagement',
          label: 'Akademik Dönem',
          path: '/faculty-management',
          icon: 'building',
        },
        {
          key: 'programOutcomes',
          label: 'Program Çıktıları',
          path: '/program-outcomes',
          icon: 'target',
        },
      ],
    },
    {
      key: 'course-student',
      title: 'Ders & Öğrenci',
      items: [
        {
          key: 'courseManagement',
          label: 'Ders Yönetimi',
          path: '/course-management',
          icon: 'book-open',
        },
        {
          key: 'studentManagement',
          label: 'Öğrenci Yönetimi',
          path: '/student-management',
          icon: 'user-round',
        },
        {
          key: 'courseStudents',
          label: 'Ders Öğrencileri',
          path: '/course-students',
          icon: 'users-round',
        },
      ],
    },
    {
      key: 'other',
      title: 'Diğer',
      items: [
        {
          key: 'surveyManagement',
          label: 'Anket Yönetimi',
          path: '/survey-management',
          icon: 'clipboard-list',
        },
      ],
    },
  ],
  home: {
    title: 'Admin Anasayfa',
    subtitle: 'Program, ders ve kullanıcı yönetimi için özet ekran.',
    stats: [
      { label: 'Program', value: '8' },
      { label: 'Ders', value: '64' },
      { label: 'Öğretmen', value: '42' },
      { label: 'Öğrenci', value: '1200' },
    ],
    highlights: [
      'GET /api/Admin/programs, program-outcomes, courses',
      'GET /api/Admin/academic-terms, course-offerings',
      'GET /api/Admin/teachers, students',
    ],
  },
  pages: {
    userManagement: {
      title: 'Kullanıcı Yönetimi',
      description:
        'API’de merkezi kullanıcı listesi yok; öğretmen ve öğrenci uçları üzerinden özet ve bağlantılar.',
    },
    teacherManagement: {
      title: 'Öğretmen Yönetimi',
      description: 'Öğretmen kayıtları ve öğretmen atama işlemleri burada yönetilecek.',
    },
    facultyManagement: {
      title: 'Akademik Dönem',
      description: 'Akademik dönem listesi ve aktif dönem (GET/PUT /api/Admin/academic-terms).',
    },
    programOutcomes: {
      title: 'Program Çıktıları',
      description: 'Program çıktıları ve ders eşleştirmeleri burada düzenlenecek.',
    },
    courseManagement: {
      title: 'Ders Yönetimi',
      description: 'Ders, CLO ve ders açılımı süreçleri bu ekranda yönetilecek.',
    },
    studentManagement: {
      title: 'Öğrenci Yönetimi',
      description: 'Öğrenci kayıt, güncelleme ve durum takibi burada yapılacak.',
    },
    courseStudents: {
      title: 'Ders Öğrencileri',
      description: 'Ders bazlı öğrenci listeleri ve toplu işlemler bu bölümde olacak.',
    },
    surveyManagement: {
      title: 'Anket Yönetimi',
      description: 'Anket oluşturma, düzenleme ve yayınlama işlemleri burada yönetilecek.',
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
    searchPlaceholder: 'Fakülte, ders veya kullanıcı ara…',
    breadcrumbAriaLabel: 'Sayfa konumu',
    breadcrumbRoot: 'MUDEK',
    profileLabel: 'Profil',
    profileMenuAriaLabel: 'Hesap menüsü',
    profileName: 'Admin Kullanıcı',
    profileRole: 'Yönetici',
    logoutLabel: 'Güvenli Çıkış',
    logoutError: 'Çıkış yapılırken bir hata oluştu.',
    homeFocusTitle: 'Bağlı API uçları',
  },
}

