import React from 'react'
import styled from 'styled-components'

const ParticleContainer = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: -1;
  pointer-events: none;
  overflow: hidden;
  background: transparent;
`

const FloatingCircle = styled.div<{ size: number; left: number; animationDelay: number; duration: number }>`
  position: absolute;
  width: ${props => props.size}px;
  height: ${props => props.size}px;
  background: linear-gradient(45deg, rgba(182, 201, 155, 0.3), rgba(152, 167, 124, 0.2));
  border-radius: 50%;
  left: ${props => props.left}%;
  animation: float ${props => props.duration}s ease-in-out infinite;
  animation-delay: ${props => props.animationDelay}s;
  
  @keyframes float {
    0%, 100% {
      transform: translateY(100vh) rotate(0deg);
      opacity: 0;
    }
    10% {
      opacity: 0.6;
    }
    90% {
      opacity: 0.6;
    }
    100% {
      transform: translateY(-100px) rotate(360deg);
      opacity: 0;
    }
  }
`

const ParticleBackground: React.FC = () => {
    // Generate random particles
    const particles = Array.from({ length: 15 }, (_, index) => ({
        id: index,
        size: Math.random() * 20 + 10,
        left: Math.random() * 100,
        animationDelay: Math.random() * 10,
        duration: Math.random() * 10 + 15
    }))

    return (
        <ParticleContainer>
            {particles.map(particle => (
                <FloatingCircle
                    key={particle.id}
                    size={particle.size}
                    left={particle.left}
                    animationDelay={particle.animationDelay}
                    duration={particle.duration}
                />
            ))}
        </ParticleContainer>
    )
}

export default ParticleBackground