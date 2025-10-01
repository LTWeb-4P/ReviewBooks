import React from 'react'
import ReactDOM from 'react-dom/client'
// import App from './App.tsx'          // Complex version with animations
import SimpleApp from './SimpleApp.tsx' // Simple version without external deps
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
        <SimpleApp />
    </React.StrictMode>,
)