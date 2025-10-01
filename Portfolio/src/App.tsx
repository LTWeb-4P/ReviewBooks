import { useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import Navbar from './components/Navbar'
import Hero from './components/Hero'
import About from './components/About'
import Skills from './components/Skills'
import Projects from './components/Projects'
import Contact from './components/Contact'
import ParticleBackground from './components/ParticleBackgroundSimple'
import ScrollToTop from './components/ScrollToTop'
import './App.css'

function App() {
    useEffect(() => {
        // Simple scroll behavior setup
        document.documentElement.style.scrollBehavior = 'smooth'
    }, [])

    return (
        <div className="App">
            <ParticleBackground />
            <AnimatePresence>
                <motion.div
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    exit={{ opacity: 0 }}
                    transition={{ duration: 0.5 }}
                >
                    <Navbar />
                    <Hero />
                    <About />
                    <Skills />
                    <Projects />
                    <Contact />
                    <ScrollToTop />
                </motion.div>
            </AnimatePresence>
        </div>
    )
}

export default App