import { useLocation, useNavigate, Navigate } from 'react-router-dom'
import ConfirmationPage from '../components/ConfirmationPage.jsx'

export default function ConfirmationPageRoute() {
  const { state } = useLocation()
  const navigate = useNavigate()

  // Guard: if arrived here without confirmation data, redirect to search
  if (!state?.confirmation) return <Navigate to="/" replace />

  function handleSearchAgain() {
    navigate('/')
  }

  return (
    <ConfirmationPage
      confirmation={state.confirmation}
      onSearchAgain={handleSearchAgain}
    />
  )
}
