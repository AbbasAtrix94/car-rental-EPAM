import axios from 'axios'

const BASE_URL = 'http://localhost:5186'

const api = axios.create({ baseURL: BASE_URL })

export async function searchCars({ pickup, from, to, category }) {
  const params = { pickup, from, to }
  if (category) params.category = category
  const response = await api.get('/cars/search', { params })
  return response.data
}

export async function bookCar({ vehicleResultId, driverName, documentType, documentNumber }) {
  const response = await api.post('/cars/book', {
    vehicleResultId,
    driverName,
    documentType,
    documentNumber,
  })
  return response.data
}

export async function getBooking(reference) {
  const response = await api.get(`/cars/booking/${reference}`)
  return response.data
}
