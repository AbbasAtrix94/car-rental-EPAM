import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import SearchPage from './pages/SearchPage.jsx'
import ResultsPage from './pages/ResultsPage.jsx'
import BookingPage from './pages/BookingPage.jsx'
import ConfirmationPage from './pages/ConfirmationPage.jsx'

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<SearchPage />} />
        <Route path="/results" element={<ResultsPage />} />
        <Route path="/booking" element={<BookingPage />} />
        <Route path="/confirmation" element={<ConfirmationPage />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  )
}
