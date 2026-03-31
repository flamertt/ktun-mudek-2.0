import { useCallback, useEffect, useMemo, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  ArrowUpRight,
  BookOpen,
  Building2,
  CalendarRange,
  ClipboardCheck,
  ClipboardList,
  GraduationCap,
  Layers,
  LayoutGrid,
  Link2,
  Sparkles,
  Target,
  Upload,
  UserRound,
  Users,
  UsersRound,
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

const statConfig = [
  { key: 'programs', label: 'Program', icon: LayoutGrid },
  { key: 'courses', label: 'Ders', icon: BookOpen },
  { key: 'teachers', label: 'Öğretmen', icon: GraduationCap },
  { key: 'students', label: 'Öğrenci', icon: Users },
]

const navIconByKey = {
  home: LayoutGrid,
  users: Users,
  'graduation-cap': GraduationCap,
  building: Building2,
  target: Target,
  layers: Layers,
  'link-2': Link2,
  'calendar-range': CalendarRange,
  upload: Upload,
  'clipboard-check': ClipboardCheck,
  'book-open': BookOpen,
  'user-round': UserRound,
  'users-round': UsersRound,
  'clipboard-list': ClipboardList,
}

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

  const quickLinks = useMemo(
    () => appConfig.navSections.flatMap((s) => s.items).filter((i) => i.path !== '/home'),
    [],
  )

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
        <section className={styles.hero} aria-labelledby="home-hero-title">
          <div className={styles.heroInner}>
            <div className={styles.heroTop}>
              <span className={styles.heroBadge}>
                <Sparkles size={14} strokeWidth={2} aria-hidden />
                Özet
              </span>
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
              Panele hoş geldiniz
            </h2>
            <p className={styles.heroLead}>
              Kayıtlar ve açılışlar canlı veriden gelir. Sol menüden modüllere geçebilir veya aşağıdaki
              kısayolları kullanabilirsiniz.
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

        <div className={styles.stats}>
          {statConfig.map(({ key, label, icon: Icon }) => (
            <article key={key} className={styles.statCard}>
              <div className={styles.statIcon} aria-hidden>
                <Icon strokeWidth={1.75} size={22} />
              </div>
              <div className={styles.statBody}>
                <p className={styles.statLabel}>{label}</p>
                <p className={styles.statValue}>{loading ? '…' : stats[key]}</p>
              </div>
            </article>
          ))}
        </div>

        <section className={styles.quickSection} aria-labelledby="quick-heading">
          <div className={styles.quickHead}>
            <h2 id="quick-heading" className={styles.quickTitle}>
              Hızlı erişim
            </h2>
            <p className={styles.quickSubtitle}>Sık kullanılan yönetim ekranları</p>
          </div>
          <div className={styles.quickGrid}>
            {quickLinks.map((item) => {
              const Icon = navIconByKey[item.icon] ?? LayoutGrid
              return (
                <Link key={item.path} className={styles.quickCard} to={item.path}>
                  <span className={styles.quickIconWrap} aria-hidden>
                    <Icon strokeWidth={1.75} size={20} />
                  </span>
                  <span className={styles.quickLabel}>{item.label}</span>
                  <ArrowUpRight className={styles.quickArrow} strokeWidth={2} size={18} aria-hidden />
                </Link>
              )
            })}
          </div>
        </section>
      </div>
    </PageSection>
  )
}
