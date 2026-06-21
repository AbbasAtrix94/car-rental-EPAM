import { useState } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import SearchForm from '../components/SearchForm.jsx'
import { searchCars } from '../api/carRentalApi.js'
import { formatApiError } from '../utils/errorUtils.js'

export default function SearchPage() {
  const navigate = useNavigate()
  const location = useLocation()

  // Carry over any redirect error passed from BookingPage (stale session)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState(location.state?.redirectError || '')

  async function handleSearch(params) {
    setError('')
    setLoading(true)
    try {
      const results = await searchCars(params)
      navigate('/results', { state: { results, searchParams: params } })
    } catch (err) {
      setError(formatApiError(err))
    } finally {
      setLoading(false)
    }
  }

  return <SearchForm onSearch={handleSearch} loading={loading} error={error} />
}
