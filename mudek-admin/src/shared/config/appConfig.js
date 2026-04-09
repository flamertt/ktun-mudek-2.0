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
    cloManagement: '/clo-management',
    cloPoMapping: '/clo-po-mapping',
    letterGradeRules: '/letter-grade-rules',
    offeringManagement: '/offerings',
    studentManagement: '/student-management',
    courseStudents: '/course-students',
    enrollmentBulk: '/enrollment-bulk',
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
        {
          key: 'cloManagement',
          label: 'Ders öğrenim çıktıları (CLO)',
          path: '/clo-management',
          icon: 'layers',
        },
        {
          key: 'cloPoMapping',
          label: 'CLO ↔ PÇ eşlemesi',
          path: '/clo-po-mapping',
          icon: 'link-2',
        },
        {
          key: 'letterGradeRules',
          label: 'Harf notu kuralları (program)',
          path: '/letter-grade-rules',
          icon: 'percent',
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
          key: 'offeringManagement',
          label: 'Ders açılışları',
          path: '/offerings',
          icon: 'calendar-range',
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
        {
          key: 'enrollmentBulk',
          label: 'Toplu kayıt / Excel',
          path: '/enrollment-bulk',
          icon: 'upload',
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
  },
  pages: {
    userManagement: {
      title: 'Kullanıcı Yönetimi',
      description: 'Öğretmen ve öğrenci kayıtlarına özet bakın; listeler için ilgili yönetim ekranlarına geçin.',
    },
    teacherManagement: {
      title: 'Öğretmen Yönetimi',
      description: 'Öğretmen kayıtları ve öğretmen atama işlemleri burada yönetilecek.',
    },
    facultyManagement: {
      title: 'Akademik Dönem',
      description: 'Akademik dönem listesi ve aktif dönem ataması.',
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
    cloManagement: {
      title: 'Ders öğrenim çıktıları (CLO)',
      description: 'Katalog dersine bağlı CLO kayıtlarını oluşturun, düzenleyin veya silin.',
    },
    cloPoMapping: {
      title: 'CLO ↔ program çıktısı eşlemesi',
      description: 'Seçilen derse ait CLO ile program çıktıları arasındaki eşleme ve ağırlıkları yönetin.',
    },
    letterGradeRules: {
      title: 'Harf notu kuralları',
      description:
        'Lisans programı bazında başarı notu aralıkları; programa bağlı tüm derslerin MÜDEK hesaplamasında kullanılır.',
    },
    offeringManagement: {
      title: 'Ders açılışları',
      description: 'Dönem bazlı ders açılışı oluşturma, öğretmen atama ve güncelleme.',
    },
    enrollmentBulk: {
      title: 'Toplu öğrenci kaydı',
      description: 'Öğrenci kimlik listesi ile toplu kayıt veya Excel dosyası ile içe aktarma.',
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
    homeFocusTitle: 'Öne çıkanlar',
  },
}

