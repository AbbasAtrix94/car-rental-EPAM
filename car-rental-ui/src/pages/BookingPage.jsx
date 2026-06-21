import { useState } from 'react'
import { useLocation, useNavigate, Navigate } from 'react-router-dom'
import BookingForm from '../components/BookingForm.jsx'
import { bookCar } from '../api/carRentalApi.js'
import { formatApiError } from '../utils/errorUtils.js'

export default function BookingPage() {
  const { state } = useLocation()
  const navigate = useNavigate()
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  if (!state?.vehicle) return <Navigate to="/" replace />

  async function handleConfirm(bookingData) {
    setError('')
    setLoading(true)
    try {
      const confirmation = await bookCar(bookingData)
      navigate('/confirmation', { state: { confirmation } })
    } catch (err) {
      const status = err?.response?.status
      if (status === 422) {
        // Vehicle result not found in store — API likely restarted, session is stale.
        // Redirect to search so the user can start fresh.
        navigate('/', { replace: true, state: { redirectError: formatApiError(err) + ' — your session expired, please search again.' } })
      } else {
        setError(formatApiError(err))
      }
    } finally {
      setLoading(false)
    }
  }

  function handleBack() {
    navigate(-1)
  }

  return (
    <BookingForm
      vehicle={state.vehicle}
      onConfirm={handleConfirm}
      onBack={handleBack}
      loading={loading}
      error={error}
    />
  )
}
