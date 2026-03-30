import styles from './LoginCard.module.css'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'

import { loginAsTeacher } from '../../../shared/api/authApi'
import { appConfig } from '../../../shared/config/appConfig'
import { authTexts } from '../../../shared/lib/texts/auth.js'

export function LoginCard() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [errorMessage, setErrorMessage] = useState('')

  const handleSubmit = async (event) => {
    event.preventDefault()

    setErrorMessage('')

    if (!email.trim() || !password) {
      setErrorMessage(authTexts.requiredError)
      return
    }

    setIsSubmitting(true)

    try {
      const response = await loginAsTeacher({
        email: email.trim(),
        password,
      })

      localStorage.setItem(appConfig.storage.tokenKey, response.token)
      localStorage.setItem(appConfig.storage.userKey, JSON.stringify(response.user))
      navigate(appConfig.routes.home)
    } catch (error) {
      setErrorMessage(error.message)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className={styles.card} aria-label={authTexts.ariaLabel}>
      <img className={styles.logo} src="/logo.png" alt={authTexts.logoAlt} />

      <h1 className={styles.title}>{authTexts.title}</h1>

      <form className={styles.form} onSubmit={handleSubmit}>
        <div className={styles.field}>
          <label className={styles.label} htmlFor="username">
            {authTexts.usernameLabel}
          </label>
          <input
            className={styles.input}
            id="username"
            name="username"
            type="email"
            autoComplete="username"
            placeholder={authTexts.usernamePlaceholder}
            value={email}
            onChange={(event) => setEmail(event.target.value)}
          />
        </div>

        <div className={styles.field}>
          <label className={styles.label} htmlFor="password">
            {authTexts.passwordLabel}
          </label>
          <input
            className={styles.input}
            id="password"
            name="password"
            type="password"
            autoComplete="current-password"
            placeholder={authTexts.passwordPlaceholder}
            value={password}
            onChange={(event) => setPassword(event.target.value)}
          />
        </div>

        <div className={styles.actionsRow}>
          <a className={styles.forgotLink} href={authTexts.forgotPasswordHref}>
            {authTexts.forgotPassword}
          </a>
        </div>

        <div className={styles.captcha} aria-label={authTexts.captchaLabel}>
          {authTexts.captchaPlaceholder}
        </div>

        <button className={styles.button} type="submit" disabled={isSubmitting}>
          {isSubmitting ? authTexts.submitting : authTexts.submit}
        </button>

        {errorMessage ? <p className={styles.errorText}>{errorMessage}</p> : null}
      </form>
    </section>
  )
}

