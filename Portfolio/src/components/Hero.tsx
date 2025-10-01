import React, { useEffect, useRef } from 'react'
import { motion } from 'framer-motion'
import Typewriter from 'typewriter-effect'
import styled from 'styled-components'

const HeroSection = styled.section`
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  overflow: hidden;
`

const HeroContent = styled.div`
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  text-align: center;
  z-index: 2;
`

const HeroTitle = styled(motion.h1)`
  font-size: 3.5rem;
  font-weight: 800;
  margin-bottom: 1rem;
  background: var(--gradient);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  
  @media (max-width: 768px) {
    font-size: 2.5rem;
  }
`

const HeroSubtitle = styled(motion.div)`
  font-size: 1.5rem;
  color: var(--text-light);
  margin-bottom: 2rem;
  min-height: 60px;
  
  .typed-cursor {
    color: var(--primary-color);
    font-weight: bold;
  }
  
  @media (max-width: 768px) {
    font-size: 1.2rem;
  }
`

const HeroDescription = styled(motion.p)`
  font-size: 1.2rem;
  color: var(--text-light);
  margin-bottom: 3rem;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
  line-height: 1.8;
  
  @media (max-width: 768px) {
    font-size: 1rem;
  }
`

const ButtonGroup = styled(motion.div)`
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
`

const HeroButton = styled(motion.a)`
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 15px 30px;
  border-radius: 50px;
  text-decoration: none;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
  
  &.primary {
    background: var(--gradient);
    color: white;
    
    &:hover {
      transform: translateY(-3px);
      box-shadow: 0 10px 30px rgba(182, 201, 155, 0.4);
    }
  }
  
  &.secondary {
    background: transparent;
    color: var(--primary-color);
    border: 2px solid var(--primary-color);
    
    &:hover {
      background: var(--primary-color);
      color: white;
      transform: translateY(-3px);
    }
  }
`

const FloatingShape = styled(motion.div)`
  position: absolute;
  border-radius: 50%;
  background: linear-gradient(45deg, var(--primary-color), var(--secondary-color));
  opacity: 0.1;
  z-index: 1;
`

const ScrollIndicator = styled(motion.div)`
  position: absolute;
  bottom: 30px;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  color: var(--text-light);
  cursor: pointer;
  
  .mouse {
    width: 25px;
    height: 40px;
    border: 2px solid var(--primary-color);
    border-radius: 20px;
    margin-bottom: 10px;
    position: relative;
    
    &::before {
      content: '';
      position: absolute;
      top: 8px;
      left: 50%;
      transform: translateX(-50%);
      width: 3px;
      height: 10px;
      background: var(--primary-color);
      border-radius: 2px;
      animation: scroll 2s infinite;
    }
  }
  
  @keyframes scroll {
    0% {
      opacity: 0;
      transform: translateX(-50%) translateY(0);
    }
    50% {
      opacity: 1;
    }
    100% {
      opacity: 0;
      transform: translateX(-50%) translateY(10px);
    }
  }
`

const Hero: React.FC = () => {
    const shapesRef = useRef<HTMLDivElement[]>([])

    useEffect(() => {
        const handleMouseMove = (e: MouseEvent) => {
            const shapes = shapesRef.current
            shapes.forEach((shape, index) => {
                if (shape) {
                    const speed = (index + 1) * 0.1
                    const x = (e.clientX * speed) / 100
                    const y = (e.clientY * speed) / 100
                    shape.style.transform = `translate(${x}px, ${y}px)`
                }
            })
        }

        window.addEventListener('mousemove', handleMouseMove)
        return () => window.removeEventListener('mousemove', handleMouseMove)
    }, [])

    const scrollToAbout = () => {
        const aboutSection = document.getElementById('about')
        if (aboutSection) {
            aboutSection.scrollIntoView({ behavior: 'smooth' })
        }
    }

    const containerVariants = {
        hidden: { opacity: 0 },
        visible: {
            opacity: 1,
            transition: {
                delayChildren: 0.3,
                staggerChildren: 0.2
            }
        }
    }

    const itemVariants = {
        hidden: { y: 20, opacity: 0 },
        visible: {
            y: 0,
            opacity: 1,
            transition: {
                duration: 0.8,
                ease: "easeOut"
            }
        }
    }

    return (
        <HeroSection id="home">
            {/* Floating Shapes */}
            {[...Array(5)].map((_, index) => (
                <FloatingShape
                    key={index}
                    ref={el => {
                        if (el) shapesRef.current[index] = el
                    }}
                    style={{
                        width: `${100 + index * 50}px`,
                        height: `${100 + index * 50}px`,
                        top: `${20 + index * 15}%`,
                        left: `${10 + index * 20}%`,
                    }}
                    animate={{
                        y: [0, -20, 0],
                        rotate: [0, 360],
                    }}
                    transition={{
                        duration: 10 + index * 2,
                        repeat: Infinity,
                        ease: "linear"
                    }}
                />
            ))}

            <HeroContent>
                <motion.div
                    variants={containerVariants}
                    initial="hidden"
                    animate="visible"
                >
                    <HeroTitle variants={itemVariants}>
                        Ngô Thanh Tân
                    </HeroTitle>

                    <HeroSubtitle variants={itemVariants}>
                        <Typewriter
                            options={{
                                strings: [
                                    "Fullstack Developer",
                                    "UI/UX Designer",
                                    "Problem Solver",
                                    "Tech Enthusiast"
                                ],
                                autoStart: true,
                                loop: true,
                                delay: 80,
                                deleteSpeed: 50,
                            }}
                        />
                    </HeroSubtitle>                    <HeroDescription variants={itemVariants}>
                        Sinh năm 2005, đam mê công nghệ và phát triển phần mềm.
                        Chuyên về Django, React, và các công nghệ web hiện đại.
                        Luôn sẵn sàng học hỏi và tạo ra những sản phẩm có giá trị.
                    </HeroDescription>

                    <ButtonGroup variants={itemVariants}>
                        <HeroButton
                            href="#projects"
                            className="primary"
                            onClick={(e) => {
                                e.preventDefault()
                                document.getElementById('projects')?.scrollIntoView({ behavior: 'smooth' })
                            }}
                            whileHover={{ scale: 1.05 }}
                            whileTap={{ scale: 0.95 }}
                        >
                            <i className="fas fa-code"></i>
                            Xem Dự Án
                        </HeroButton>

                        <HeroButton
                            href="#contact"
                            className="secondary"
                            onClick={(e) => {
                                e.preventDefault()
                                document.getElementById('contact')?.scrollIntoView({ behavior: 'smooth' })
                            }}
                            whileHover={{ scale: 1.05 }}
                            whileTap={{ scale: 0.95 }}
                        >
                            <i className="fas fa-envelope"></i>
                            Liên Hệ
                        </HeroButton>
                    </ButtonGroup>
                </motion.div>
            </HeroContent>

            <ScrollIndicator
                onClick={scrollToAbout}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 2, duration: 0.8 }}
            >
                <div className="mouse"></div>
                <span>Cuộn xuống</span>
            </ScrollIndicator>
        </HeroSection>
    )
}

export default Hero