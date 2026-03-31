import { useMemo } from 'react'
import {
  CalendarRange,
  ClipboardCheck,
  GraduationCap,
  Sparkles,
  Target,
  Upload,
  UsersRound,
} from 'lucide-react'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { appConfig } from '../../../shared/config/appConfig'
import styles from './HomePage.module.css'

export function HomePage() {
  const heroStats = useMemo(() => appConfig.home.stats.slice(0, 2), [])

  const activityItems = useMemo(() => {
    return [
      {
        icon: GraduationCap,
        title: 'Ders açılışı güncellendi',
        time: '5 dk önce',
        desc: 'Aktif dönem ders kontenjanı ve öğretmen ataması güncellendi.',
      },
      {
        icon: ClipboardCheck,
        title: 'Değerlendirme kaydı oluşturuldu',
        time: '22 dk önce',
        desc: 'Yeni ders değerlendirme kaydı başarıyla oluşturuldu.',
      },
      {
        icon: CalendarRange,
        title: 'Sınav planı revize edildi',
        time: '1 saat önce',
        desc: 'Ara sınav tarih/saat bilgileri düzenlendi.',
      },
      {
        icon: Target,
        title: 'CLO eşleme değişikliği yapıldı',
        time: 'Dün',
        desc: 'Soru ve bileşen bazlı CLO ağırlıkları güncellendi.',
      },
      {
        icon: Upload,
        title: 'Not girişi tamamlandı',
        time: '2 gün önce',
        desc: 'Bileşen puanları sisteme toplu olarak yüklendi.',
      },
      {
        icon: Sparkles,
        title: 'MÜDEK snapshot yenilendi',
        time: '3 gün önce',
        desc: 'Başarı setleri ve sonuç tabloları son hesapla ile güncellendi.',
      },
      {
        icon: ClipboardCheck,
        title: 'Harf notu kuralları güncellendi',
        time: '4 gün önce',
        desc: 'Geçme notu eşiği ve harf aralıkları revize edildi.',
      },
      {
        icon: Target,
        title: 'Program çıktısı katkısı hesaplandı',
        time: '5 gün önce',
        desc: 'PÇ seviyesinde ağırlıklı başarı oranları yeniden üretildi.',
      },
      {
        icon: GraduationCap,
        title: 'Ders öğrenci listesi yenilendi',
        time: '6 gün önce',
        desc: 'Yeni kayıtlı öğrenciler ders listesine eklendi.',
      },
      {
        icon: CalendarRange,
        title: 'Bütünleme sınavı planlandı',
        time: '1 hafta önce',
        desc: 'Bütünleme sınavı tarih ve salon bilgileri tanımlandı.',
      },
      {
        icon: Upload,
        title: 'Bileşen puanları içe aktarıldı',
        time: '1 hafta önce',
        desc: 'Proje ve ödev bileşen notları toplu dosyadan işlendi.',
      },
      {
        icon: Sparkles,
        title: 'CLO sonuç trendi oluşturuldu',
        time: '8 gün önce',
        desc: 'Dönem içi CLO başarı değişimleri rapora eklendi.',
      },
    ]
  }, [])

  const calendarItems = useMemo(
    () => [
      { month: 'Eki', day: '18', title: 'Sınav haftası başlıyor', sub: 'Tüm dersler' },
      { month: 'Eki', day: '25', title: 'Kayıt son tarihi', sub: 'Geç başvurular' },
      { month: 'Kas', day: '02', title: 'Öğretmen toplantısı', sub: 'Ana salon' },
    ],
    [],
  )

  const announcementItems = useMemo(
    () => [
      { time: 'Bugün · 09:15', title: 'Kampus Wi-Fi bakım planı', desc: '2:00 - 4:00 arası kesinti bekleniyor.' },
      { time: 'Dün', title: 'Araştırma destek duyurusu', desc: '2023 sonuç listesi yayınlandı.' },
      { time: '12 Eki 2023', title: 'Kütüphane çalışma saatleri', desc: 'Güncel açılış saatleri aktif oldu.' },
    ],
    [],
  )

  return (
    <PageSection title={appConfig.home.title} description={appConfig.home.subtitle} loading={false}>
      <div className={styles.layout}>
        <div className={styles.dashboardGrid}>
          <div className={styles.leftCol}>
            <section className={styles.hero} aria-labelledby="home-hero-title">
              <div className={styles.heroInner}>
                <div className={styles.heroTop}>
                 
                  <span className={styles.termPillMuted}>Aktif dönem</span>
                </div>

                <h2 id="home-hero-title" className={styles.heroTitle}>
                  Hoş geldiniz
                </h2>
                <p className={styles.heroLead}>
                  Ders, sınav ve değerlendirme süreçlerini tek ekranda takip edin.
                </p>

                <div className={styles.heroMetrics}>
                  {heroStats.map((s) => (
                    <div key={s.label} className={styles.heroMetric}>
                      <span className={styles.heroMetricLabel}>{s.label}</span>
                      <span className={styles.heroMetricValue}>{s.value}</span>
                    </div>
                  ))}
                </div>
              </div>
            </section>

            <section className={styles.sideCard} aria-labelledby="activity-heading">
              <div className={styles.sideHeader}>
                <div className={styles.sideHeaderLeft}>
                  <span className={styles.sideHeaderIcon} aria-hidden>
                    <UsersRound strokeWidth={1.75} size={18} />
                  </span>
                  <h3 id="activity-heading" className={styles.sideTitle}>
                    Son aktiviteler
                  </h3>
                </div>
                <span className={styles.sideLink}>Tümünü gör</span>
              </div>

              <div className={styles.activityList}>
                {activityItems.map((item) => {
                  const Icon = item.icon
                  return (
                    <div key={item.title} className={styles.activityItem}>
                      <div className={styles.activityIcon} aria-hidden>
                        <Icon strokeWidth={1.75} size={18} />
                      </div>
                      <div className={styles.activityBody}>
                        <div className={styles.activityTopLine}>
                          <span className={styles.activityTitle}>{item.title}</span>
                          <span className={styles.activityTime}>{item.time}</span>
                        </div>
                        <p className={styles.activityDesc}>{item.desc}</p>
                      </div>
                    </div>
                  )
                })}
              </div>
            </section>
          </div>

          <div className={styles.rightCol}>
            <section className={styles.sideCard} aria-labelledby="calendar-heading">
              <div className={styles.sideHeader}>
                <div className={styles.sideHeaderLeft}>
                  <span className={styles.sideHeaderIconPrimary} aria-hidden>
                    <CalendarRange strokeWidth={1.75} size={18} />
                  </span>
                  <h3 id="calendar-heading" className={styles.sideTitle}>
                    Akademik takvim
                  </h3>
                </div>
                <span className={styles.smallBadge}>Aktif</span>
              </div>

              <div className={styles.calendarList}>
                {calendarItems.map((item) => (
                  <div key={`${item.month}-${item.day}-${item.title}`} className={styles.calendarItem}>
                    <div className={styles.calendarDate}>
                      <span className={styles.calendarMonth}>{item.month}</span>
                      <span className={styles.calendarDay}>{item.day}</span>
                    </div>
                    <div>
                      <p className={styles.calendarTitle}>{item.title}</p>
                      <p className={styles.calendarSub}>{item.sub}</p>
                    </div>
                  </div>
                ))}
              </div>

              <button type="button" className={styles.sideButton} aria-label="Takvimi görüntüle">
                Takvimi görüntüle
              </button>
            </section>

            <section className={styles.sideCard} aria-labelledby="ann-heading">
              <div className={styles.sideHeader}>
                <h3 id="ann-heading" className={styles.sideTitle}>
                  Duyurular
                </h3>
                <span className={styles.latestBadge}>LATEST</span>
              </div>

              <div className={styles.annList}>
                {announcementItems.map((item, idx) => (
                  <div key={item.title} className={styles.annItem}>
                    <div className={idx === 0 ? styles.annDot : styles.annDotMuted} aria-hidden />
                    <div>
                      <p className={idx === 0 ? styles.annTime : styles.annTimeMuted}>{item.time}</p>
                      <p className={styles.annTitle}>{item.title}</p>
                      <p className={styles.annDesc}>{item.desc}</p>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            <section className={styles.quoteCard} aria-label="Alıntı">
              <p className={styles.quoteText}>“Eğitimin kökleri acıdır, ama meyvesi tatlıdır.”</p>
              <p className={styles.quoteAuthor}>— Aristoteles</p>
            </section>
          </div>
        </div>
      </div>
    </PageSection>
  )
}
