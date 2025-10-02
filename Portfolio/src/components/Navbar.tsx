import React, { useState, useEffect } from 'react'
import { motion } from 'framer-motion'
import styled from 'styled-components'

const NavbarContainer = styled(motion.nav)`
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  background: ${props => props.scrolled ? 'rgba(255, 255, 255, 0.95)' : 'transparent'};
  backdrop-filter: ${props => props.scrolled ? 'blur(10px)' : 'none'};
  transition: all 0.3s ease;
  padding: 1rem 0;
  border-bottom: ${props => props.scrolled ? '1px solid rgba(182, 201, 155, 0.2)' : 'none'};
`

const NavContent = styled.div`
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
`

const Logo = styled(motion.a)`
  font-size: 1.8rem;
  font-weight: 700;
  color: var(--primary-color);
  text-decoration: none;
  background: var(--gradient);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
`

const NavMenu = styled(motion.ul)`
  display: flex;
  list-style: none;
  gap: 2rem;
  
  @media (max-width: 768px) {
    position: fixed;
    top: 80px;
    left: ${props => props.isOpen ? '0' : '-100%'};
    width: 100%;
    height: calc(100vh - 80px);
    background: rgba(255, 255, 255, 0.98);
    backdrop-filter: blur(10px);
    flex-direction: column;
    justify-content: flex-start;
    align-items: center;
    padding-top: 2rem;
    transition: left 0.3s ease;
  }
`

const NavItem = styled(motion.li)`
  position: relative;
`

const NavLink = styled(motion.a)`
  color: var(--text-dark);
  text-decoration: none;
  font-weight: 500;
  font-size: 1rem;
  position: relative;
  transition: color 0.3s ease;
  
  &:hover {
    color: var(--primary-color);
  }
  
  &.active {
    color: var(--primary-color);
  }
  
  &::after {
    content: '';
    position: absolute;
    bottom: -5px;
    left: 0;
    width: ${props => props.active ? '100%' : '0'};
    height: 2px;
    background: var(--gradient);
    transition: width 0.3s ease;
  }
  
  &:hover::after {
    width: 100%;
  }
`

const MobileToggle = styled.div`
  display: none;
  flex-direction: column;
  cursor: pointer;
  
  @media (max-width: 768px) {
    display: flex;
  }
  
  span {
    width: 25px;
    height: 3px;
    background: var(--primary-color);
    margin: 3px 0;
    transition: 0.3s;
    transform-origin: center;
    
    &:nth-child(1) {
      transform: ${props => props.isOpen ? 'rotate(45deg) translate(6px, 6px)' : 'none'};
    }
    
    &:nth-child(2) {
      opacity: ${props => props.isOpen ? '0' : '1'};
    }
    
    &:nth-child(3) {
      transform: ${props => props.isOpen ? 'rotate(-45deg) translate(6px, -6px)' : 'none'};
    }
  }
`

const Navbar: React.FC = () => {
  const [scrolled, setScrolled] = useState(false)
  const [activeSection, setActiveSection] = useState('home')
  const [mobileOpen, setMobileOpen] = useState(false)

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 50)

      // Update active section based on scroll position
      const sections = ['home', 'about', 'skills', 'projects', 'contact']
      const scrollPosition = window.scrollY + 100

      for (const section of sections) {
        const element = document.getElementById(section)
        if (element) {
          const offsetTop = element.offsetTop
          const offsetHeight = element.offsetHeight

          if (scrollPosition >= offsetTop && scrollPosition < offsetTop + offsetHeight) {
            setActiveSection(section)
            break
          }
        }
      }
    }

    window.addEventListener('scroll', handleScroll)
    return () => window.removeEventListener('scroll', handleScroll)
  }, [])

  const scrollToSection = (sectionId: string) => {
    const element = document.getElementById(sectionId)
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' })
      setMobileOpen(false)
    }
  }

  const navItems = [
    { id: 'home', label: 'Trang Chủ' },
    { id: 'about', label: 'Giới Thiệu' },
    { id: 'skills', label: 'Kỹ Năng' },
    { id: 'projects', label: 'Dự Án' },
    { id: 'contact', label: 'Liên Hệ' }
  ]

  return (
    <NavbarContainer
      scrolled={scrolled}
      initial={{ y: -100 }}
      animate={{ y: 0 }}
      transition={{ duration: 0.5 }}
    >
      <NavContent>
        <Logo
          href="#section0"
          onClick={(e) => {
            e.preventDefault()
            scrollToSection('home')
          }}
          whileHover={{ scale: 1.05 }}
          whileTap={{ scale: 0.95 }}
        >
          Portfolio
        </Logo>

        <NavMenu isOpen={mobileOpen}>
          {navItems.map((item, index) => (
            <NavItem
              key={item.id}
              initial={{ opacity: 0, y: -20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: index * 0.1 }}
            >
              <NavLink
                href={`#${item.id}`}
                active={activeSection === item.id}
                onClick={(e) => {
                  e.preventDefault()
                  scrollToSection(item.id)
                }}
                whileHover={{ y: -2 }}
                whileTap={{ y: 0 }}
              >
                {item.label}
              </NavLink>
            </NavItem>
          ))}
        </NavMenu>

        <MobileToggle
          isOpen={mobileOpen}
          onClick={() => setMobileOpen(!mobileOpen)}
        >
          <span></span>
          <span></span>
          <span></span>
        </MobileToggle>
      </NavContent>
    </NavbarContainer>
  )
}

export default Navbar