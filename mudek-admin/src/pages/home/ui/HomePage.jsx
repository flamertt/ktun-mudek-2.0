import { useCallback, useEffect, useMemo, useState } from 'react'
import {
  Sparkles,
  CalendarRange,
  Upload,
  UsersRound,
  GraduationCap,
} from 'lucide-react'

import {
  fetchActiveAcademicTerm,
  fetchCourseOfferingsActiveTerm,
  fetchCourses,
  fetchPrograms,
  fetchStudents,
  fetchTeachers,
} from '../../../shared/api/adminApi'
import { getAdminToken } from '../../../shared/lib/authToken'
import { appConfig } from '../../../shared/config/appConfig'
import { PageSection } from '@shared/ui/page-section/PageSection.jsx'
import { RefreshIconButton } from '../../../shared/ui/refresh-icon-button/RefreshIconButton.jsx'
import styles from './HomePage.module.css'

export function HomePage() {
  const [stats, setStats] = useState({
    programs: '—',
    courses: '—',
    teachers: '—',
    students: '—',
  })
  const [activeTermName, setActiveTermName] = useState(null)
  const [offeringsCount, setOfferingsCount] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  const activityItems = useMemo(
    () => [
      {
        icon: UsersRound,
        title: 'Kayıtlar güncellendi',
        time: '10 dk önce',
        desc: 'Öğrenci kayıt sayıları aktif dönem için yeniden hesaplandı.',
      },
      {
        icon: GraduationCap,
        title: 'Öğretmen profili oluşturuldu',
        time: '2 saat önce',
        desc: 'Yeni öğretmen hesabı ve birim bilgileri sisteme eklendi.',
      },
      {
        icon: Upload,
        title: 'Excel içe aktarma çalıştı',
        time: 'Dün',
        desc: 'Toplu öğrenci aktarımında 128 yeni kayıt doğrulandı.',
      },
      {
        icon: CalendarRange,
        title: 'Ders açılışı onaylandı',
        time: 'Dün',
        desc: '2026 Bahar dönemi için 6 yeni ders açılışı aktif edildi.',
      },
      {
        icon: Sparkles,
        title: 'Program çıktısı seti güncellendi',
        time: '3 gün önce',
        desc: 'Mühendislik programı PÇ setinde sürüm yükseltmesi yapıldı.',
      },
      {
        icon: UsersRound,
        title: 'Ders-öğrenci eşlemesi tamamlandı',
        time: '4 gün önce',
        desc: 'Toplu kayıt ekranından ders atama işlemleri başarıyla bitti.',
      },
      {
        icon: GraduationCap,
        title: 'Öğretmen ataması revize edildi',
        time: '5 gün önce',
        desc: 'Aktif dönem için 3 ders açılışında öğretmen değişikliği yapıldı.',
      },
      {
        icon: Upload,
        title: 'Toplu kayıt doğrulaması tamamlandı',
        time: '6 gün önce',
        desc: 'Hatalı satırlar ayıklanarak öğrenci listesi temizlendi.',
      },
      {
        icon: CalendarRange,
        title: 'Akademik dönem tarihleri güncellendi',
        time: '1 hafta önce',
        desc: 'Dönem başlangıç ve bitiş tarihleri yönetimce onaylandı.',
      },
      {
        icon: Sparkles,
        title: 'Program çıktısı eşleme raporu üretildi',
        time: '1 hafta önce',
        desc: 'CLO ↔ PÇ kapsama oranları rapor tablosuna eklendi.',
      },
      {
        icon: UsersRound,
        title: 'Yeni öğrenci grubu oluşturuldu',
        time: '9 gün önce',
        desc: 'Yatay geçiş öğrencileri için ayrı kayıt grubu tanımlandı.',
      },
      {
        icon: GraduationCap,
        title: 'Danışmanlık rolü atandı',
        time: '10 gün önce',
        desc: 'Bölüm bazlı danışman öğretmen rolleri aktifleştirildi.',
      },
    ],
    [],
  )

  const termShort = activeTermName ? activeTermName.split(' ')[0] : 'Aktif'

  const refreshDashboard = useCallback(async () => {
    const token = getAdminToken()
    if (!token) {
      setLoading(false)
      return
    }
    setError('')
    setLoading(true)
    try {
      const [programs, courses, teachers, students, activeTerm, offerings] = await Promise.all([
        fetchPrograms(token),
        fetchCourses(token),
        fetchTeachers(token),
        fetchStudents(token),
        fetchActiveAcademicTerm(token).catch(() => null),
        fetchCourseOfferingsActiveTerm(token).catch(() => []),
      ])
      setStats({
        programs: String(Array.isArray(programs) ? programs.length : 0),
        courses: String(Array.isArray(courses) ? courses.length : 0),
        teachers: String(Array.isArray(teachers) ? teachers.length : 0),
        students: String(Array.isArray(students) ? students.length : 0),
      })
      setActiveTermName(activeTerm?.name?.trim() || null)
      setOfferingsCount(Array.isArray(offerings) ? offerings.length : 0)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Özet yüklenemedi.')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void refreshDashboard()
  }, [refreshDashboard])

  return (
    <PageSection
      title={appConfig.home.title}
      description={appConfig.home.subtitle}
      error={error}
      loading={false}
    >
      <div className={styles.layout}>
        <div className={styles.dashboardGrid}>
          <div className={styles.leftCol}>
            <section className={styles.hero} aria-labelledby="home-hero-title">
              <div className={styles.heroInner}>
                <div className={styles.heroTop}>
               
                  {activeTermName ? (
                    <span className={styles.termPill}>{activeTermName}</span>
                  ) : (
                    <span className={styles.termPillMuted}>Aktif dönem bilgisi yok</span>
                  )}
                  <span className={styles.heroTopEnd}>
                    <RefreshIconButton onClick={() => void refreshDashboard()} loading={loading} />
                  </span>
                </div>
                <h2 id="home-hero-title" className={styles.heroTitle}>
                  Hoş geldiniz
                </h2>
                <p className={styles.heroLead}>
                  {termShort} dönemi için canlı özet ve hızlı erişim.
                  Sol menüden modüllere geçebilir veya aşağıdaki kısayolları kullanabilirsiniz.
                </p>
                <div className={styles.heroMetrics}>
                  <div className={styles.heroMetric}>
                    <span className={styles.heroMetricLabel}>Aktif dönem açılışı</span>
                    <span className={styles.heroMetricValue}>
                      {loading ? '…' : offeringsCount != null ? String(offeringsCount) : '—'}
                    </span>
                  </div>
                  <div className={styles.heroMetric}>
                    <span className={styles.heroMetricLabel}>Toplam öğrenci</span>
                    <span className={styles.heroMetricValue}>{loading ? '…' : stats.students}</span>
                  </div>
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
                <span className={styles.smallBadge}>{termShort}</span>
              </div>

              <div className={styles.calendarList}>
                <div className={styles.calendarItem}>
                  <div className={styles.calendarDate}>
                    <span className={styles.calendarMonth}>Eki</span>
                    <span className={styles.calendarDay}>18</span>
                  </div>
                  <div>
                    <p className={styles.calendarTitle}>Ara sınavlar başlıyor</p>
                    <p className={styles.calendarSub}>Tüm bölümler</p>
                  </div>
                </div>
                <div className={styles.calendarItem}>
                  <div className={styles.calendarDate}>
                    <span className={styles.calendarMonth}>Eki</span>
                    <span className={styles.calendarDay}>25</span>
                  </div>
                  <div>
                    <p className={styles.calendarTitle}>Kayıt son tarihi</p>
                    <p className={styles.calendarSub}>Geç başvurular</p>
                  </div>
                </div>
                <div className={styles.calendarItem}>
                  <div className={styles.calendarDate}>
                    <span className={styles.calendarMonth}>Kas</span>
                    <span className={styles.calendarDay}>02</span>
                  </div>
                  <div>
                    <p className={styles.calendarTitle}>Fakülte toplantısı</p>
                    <p className={styles.calendarSub}>Ana salon</p>
                  </div>
                </div>
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
                <div className={styles.annItem}>
                  <div className={styles.annDot} aria-hidden />
                  <div>
                    <p className={styles.annTime}>Bugün · 09:15</p>
                    <p className={styles.annTitle}>Kampus Wi-Fi bakım planı</p>
                    <p className={styles.annDesc}>2:00 - 4:00 arasında kesinti bekleniyor.</p>
                  </div>
                </div>
                <div className={styles.annItem}>
                  <div className={styles.annDotMuted} aria-hidden />
                  <div>
                    <p className={styles.annTimeMuted}>Dün</p>
                    <p className={styles.annTitle}>Araştırma destek duyurusu</p>
                    <p className={styles.annDesc}>2023 sonuç listesi yayınlandı.</p>
                  </div>
                </div>
                <div className={styles.annItem}>
                  <div className={styles.annDotMuted} aria-hidden />
                  <div>
                    <p className={styles.annTimeMuted}>12 Eki 2023</p>
                    <p className={styles.annTitle}>Kütüphane çalışma saatleri</p>
                    <p className={styles.annDesc}>Güncel açılış saatleri aktif oldu.</p>
                  </div>
                </div>
              </div>
            </section>

            <section className={styles.quoteCard} aria-label="Alıntı">
              <p className={styles.quoteText}>
                “Eğitimin kökleri acıdır, ama meyvesi tatlıdır.”
              </p>
              <p className={styles.quoteAuthor}>— Aristoteles</p>
            </section>
          </div>
        </div>
      </div>
    </PageSection>
  )
}
