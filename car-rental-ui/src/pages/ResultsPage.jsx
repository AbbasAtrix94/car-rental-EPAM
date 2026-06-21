import { useLocation, useNavigate, Navigate } from 'react-router-dom'
import ResultsList from '../components/ResultsList.jsx'

export default function ResultsPage() {
  const { state } = useLocation()
  const navigate = useNavigate()

  // Guard: if arrived here without search state, redirect to search
  if (!state?.results) return <Navigate to="/" replace />

  function handleBook(vehicle) {
    navigate('/booking', { state: { vehicle } })
  }

  function handleBack() {
    navigate('/')
  }

  return (
    <ResultsList
      results={state.results}
      searchParams={state.searchParams}
      onBook={handleBook}
      onBack={handleBack}
    />
  )
}
