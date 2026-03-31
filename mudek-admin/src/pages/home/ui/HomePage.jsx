import { useEffect, useState } from 'react'

import {
  fetchCourses,
  fetchPrograms,
  fetchStudents,
  fetchTeachers,
} from '../../../shared/api/adminApi'
import { getAdminToken } from '../../../shared/lib/authToken'
import { appConfig } from '../../../shared/config/appConfig'
import styles from './HomePage.module.css'

export function HomePage() {
  const [stats, setStats] = useState({
    programs: '—',
    courses: '—',
    teachers: '—',
    students: '—',
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const token = getAdminToken()
    if (!token) {
      setLoading(false)
      return
    }

    let cancelled = false

    async function load() {
      setError('')
      setLoading(true)
      try {
        const [programs, courses, teachers, students] = await Promise.all([
          fetchPrograms(token),
          fetchCourses(token),
          fetchTeachers(token),
          fetchStudents(token),
        ])
        if (cancelled) return
        setStats({
          programs: String(Array.isArray(programs) ? programs.length : 0),
          courses: String(Array.isArray(courses) ? courses.length : 0),
          teachers: String(Array.isArray(teachers) ? teachers.length : 0),
          students: String(Array.isArray(students) ? students.length : 0),
        })
      } catch (e) {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Özet yüklenemedi.')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }

    load()
    return () => {
      cancelled = true
    }
  }, [])

  const statItems = [
    { label: 'Program', value: stats.programs },
    { label: 'Ders', value: stats.courses },
    { label: 'Öğretmen', value: stats.teachers },
    { label: 'Öğrenci', value: stats.students },
  ]

  return (
    <section className={styles.root}>
      <header className={styles.hero}>
        <div>
          <h2 className={styles.heading}>{appConfig.home.title}</h2>
          <p className={styles.sub}>{appConfig.home.subtitle}</p>
        </div>
      </header>

      {error ? (
        <p className={styles.apiError} role="alert">
          {error}
        </p>
      ) : null}

      <div className={styles.stats}>
        {statItems.map((stat) => (
          <article key={stat.label} className={styles.card}>
            <p className={styles.statLabel}>{stat.label}</p>
            <p className={styles.statValue}>{loading ? '…' : stat.value}</p>
          </article>
        ))}
      </div>

      <article className={styles.list}>
        <h3 className={styles.listTitle}>API bağlantısı</h3>
        <p className={styles.listItem}>
          Özet sayılar{' '}
          <code className={styles.code}>GET /api/Admin/programs</code>,{' '}
          <code className={styles.code}>courses</code>, <code className={styles.code}>teachers</code>,{' '}
          <code className={styles.code}>students</code> uçlarından alınır.
        </p>
      </article>
    </section>
  )
}
