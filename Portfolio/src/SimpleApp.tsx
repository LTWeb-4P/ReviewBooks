import { useState, useEffect } from 'react'
import './App.css'

// Modern UI/UX Design System with Advanced Animations
const animations = `
@keyframes float0 {
    0%, 100% { transform: translateY(0px) rotate(0deg); }
    50% { transform: translateY(-12px) rotate(180deg); }
}

@keyframes float1 {
    0%, 100% { transform: translateY(0px) translateX(0px); }
    33% { transform: translateY(-10px) translateX(6px); }
    66% { transform: translateY(6px) translateX(-6px); }
}

@keyframes float2 {
    0%, 100% { transform: translateY(0px) scale(1); }
    50% { transform: translateY(-8px) scale(1.1); }
}

@keyframes gradientShift {
    0% { background-position: 0% 50%; }
    50% { background-position: 100% 50%; }
    100% { background-position: 0% 50%; }
}

@keyframes morphGlow {
    0%, 100% { 
        box-shadow: 0 0 20px rgba(99, 102, 241, 0.3), 0 0 40px rgba(139, 92, 246, 0.2);
        transform: scale(1);
    }
    50% { 
        box-shadow: 0 0 30px rgba(236, 72, 153, 0.4), 0 0 60px rgba(59, 130, 246, 0.3);
        transform: scale(1.05);
    }
}

@keyframes slideInLeft {
    from { transform: translateX(-60px); opacity: 0; }
    to { transform: translateX(0); opacity: 1; }
}

@keyframes slideInRight {
    from { transform: translateX(60px); opacity: 0; }
    to { transform: translateX(0); opacity: 1; }
}

@keyframes fadeInUp {
    from { transform: translateY(40px); opacity: 0; }
    to { transform: translateY(0); opacity: 1; }
}

@keyframes shimmer {
    0% { background-position: -200% 0; }
    100% { background-position: 200% 0; }
}

@keyframes colorWave {
    0% { filter: hue-rotate(0deg) saturate(1) brightness(1); }
    25% { filter: hue-rotate(90deg) saturate(1.2) brightness(1.1); }
    50% { filter: hue-rotate(180deg) saturate(1.4) brightness(1.2); }
    75% { filter: hue-rotate(270deg) saturate(1.2) brightness(1.1); }
    100% { filter: hue-rotate(360deg) saturate(1) brightness(1); }
}

@keyframes glowPulse {
    0%, 100% { 
        box-shadow: 0 0 15px rgba(99, 102, 241, 0.4);
        transform: scale(1);
    }
    50% { 
        box-shadow: 0 0 25px rgba(139, 92, 246, 0.6);
        transform: scale(1.02);
    }
}

.profile-card:hover .shine-effect {
    opacity: 1 !important;
    transform: translateX(100%) translateY(100%) rotate(45deg) !important;
}

.floating-stat {
    animation: pulse 2s ease-in-out infinite;
}

.floating-stat:nth-child(2) {
    animation-delay: 0.5s;
}

.about-content {
    animation: slideInRight 0.8s ease-out;
}

.about-image {
    animation: slideInLeft 0.8s ease-out;
}

.highlight-card {
    animation: fadeInUp 0.6s ease-out;
    animation-fill-mode: both;
}

.highlight-card:nth-child(1) { animation-delay: 0.1s; }
.highlight-card:nth-child(2) { animation-delay: 0.2s; }
.highlight-card:nth-child(3) { animation-delay: 0.3s; }
.highlight-card:nth-child(4) { animation-delay: 0.4s; }

@media (max-width: 768px) {
    .about-grid {
        grid-template-columns: 1fr !important;
        gap: 2rem !important;
        text-align: center !important;
    }
    .about-content {
        text-align: center !important;
    }
    .highlight-grid {
        grid-template-columns: 1fr !important;
    }
}
`;

// Inject animations into head
if (typeof document !== 'undefined') {
    const styleSheet = document.createElement('style');
    styleSheet.textContent = animations;
    document.head.appendChild(styleSheet);
}

// Simple components without external dependencies
const SimpleNavbar = () => {
    const [scrolled, setScrolled] = useState(false)

    useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 50)
        window.addEventListener('scroll', handleScroll)
        return () => window.removeEventListener('scroll', handleScroll)
    }, [])

    return (
        <nav style={{
            position: 'fixed',
            top: 0,
            left: 0,
            right: 0,
            background: scrolled
                ? 'rgba(255, 255, 255, 0.95)'
                : 'linear-gradient(135deg, rgba(99, 102, 241, 0.1) 0%, rgba(139, 92, 246, 0.05) 100%)',
            backdropFilter: 'blur(20px)',
            borderBottom: scrolled ? '1px solid rgba(99, 102, 241, 0.1)' : 'none',
            padding: '1rem 2rem',
            zIndex: 1000,
            transition: 'all 0.4s cubic-bezier(0.25, 0.8, 0.25, 1)'
        }}>
            <div style={{
                maxWidth: '1400px',
                margin: '0 auto',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center'
            }}>
                <a href="#home" style={{
                    fontSize: '1.8rem',
                    fontWeight: '700',
                    background: 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 50%, #ec4899 100%)',
                    WebkitBackgroundClip: 'text',
                    WebkitTextFillColor: 'transparent',
                    textDecoration: 'none',
                    letterSpacing: '-0.02em'
                }}>
                    NTT.dev
                </a>
                <div style={{ display: 'flex', gap: '2.5rem' }}>
                    {['Trang Chủ', 'Giới Thiệu', 'Kỹ Năng', 'Dự Án', 'Liên Hệ'].map((item, index) => (
                        <a
                            key={index}
                            href={`#section${index}`}
                            style={{
                                color: '#1f2937',
                                textDecoration: 'none',
                                fontWeight: '600',
                                fontSize: '0.95rem',
                                position: 'relative',
                                padding: '0.5rem 1rem',
                                borderRadius: '25px',
                                transition: 'all 0.3s cubic-bezier(0.25, 0.8, 0.25, 1)',
                                background: 'transparent'
                            }}
                            onMouseEnter={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.background = 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)';
                                target.style.color = 'white';
                                target.style.transform = 'translateY(-2px)';
                                target.style.boxShadow = '0 10px 25px rgba(99, 102, 241, 0.3)';
                            }}
                            onMouseLeave={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.background = 'transparent';
                                target.style.color = '#1f2937';
                                target.style.transform = 'translateY(0)';
                                target.style.boxShadow = 'none';
                            }}
                        >
                            {item}
                        </a>
                    ))}
                </div>
            </div>
        </nav>
    )
}

const SimpleHero = () => {
    return (
        <section id="section0" style={{
            minHeight: '100vh',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            background: `
                radial-gradient(circle at 20% 50%, rgba(99, 102, 241, 0.25) 0%, rgba(139, 92, 246, 0.1) 50%, transparent 70%),
                radial-gradient(circle at 80% 20%, rgba(236, 72, 153, 0.2) 0%, rgba(59, 130, 246, 0.1) 50%, transparent 70%),
                radial-gradient(circle at 40% 80%, rgba(139, 92, 246, 0.15) 0%, rgba(236, 72, 153, 0.1) 50%, transparent 70%),
                linear-gradient(135deg, #faf5ff 0%, #f0f4ff 50%, #fef7f7 100%)
            `,
            textAlign: 'center',
            padding: '2rem',
            position: 'relative',
            overflow: 'hidden'
        }}>
            {/* Modern floating elements */}
            <div style={{
                position: 'absolute',
                top: '15%',
                left: '8%',
                width: '120px',
                height: '120px',
                background: 'linear-gradient(135deg, rgba(99, 102, 241, 0.3) 0%, rgba(139, 92, 246, 0.4) 50%, rgba(236, 72, 153, 0.2) 100%)',
                borderRadius: '30% 70% 70% 30% / 30% 30% 70% 70%',
                animation: 'float0 8s ease-in-out infinite, morphGlow 6s ease-in-out infinite, colorWave 12s linear infinite',
                boxShadow: '0 10px 30px rgba(99, 102, 241, 0.3)'
            }}></div>
            <div style={{
                position: 'absolute',
                top: '25%',
                right: '12%',
                width: '80px',
                height: '80px',
                background: 'linear-gradient(135deg, rgba(236, 72, 153, 0.4) 0%, rgba(59, 130, 246, 0.5) 50%, rgba(99, 102, 241, 0.3) 100%)',
                borderRadius: '50% 50% 50% 50% / 60% 60% 40% 40%',
                animation: 'float1 6s ease-in-out infinite, colorWave 8s linear infinite, glowPulse 5s ease-in-out infinite',
                boxShadow: '0 8px 25px rgba(236, 72, 153, 0.4)'
            }}></div>
            <div style={{
                position: 'absolute',
                bottom: '20%',
                left: '15%',
                width: '100px',
                height: '100px',
                background: 'linear-gradient(135deg, rgba(139, 92, 246, 0.3) 0%, rgba(99, 102, 241, 0.4) 50%, rgba(236, 72, 153, 0.25) 100%)',
                borderRadius: '40% 60% 60% 40% / 60% 30% 70% 40%',
                animation: 'float2 7s ease-in-out infinite, gradientShift 4s ease-in-out infinite, morphGlow 6s ease-in-out infinite',
                boxShadow: '0 12px 30px rgba(139, 92, 246, 0.3)'
            }}></div>
            <div style={{
                position: 'absolute',
                bottom: '30%',
                right: '8%',
                width: '140px',
                height: '140px',
                background: 'linear-gradient(135deg, rgba(59, 130, 246, 0.4) 0%, rgba(236, 72, 153, 0.5) 50%, rgba(139, 92, 246, 0.3) 100%)',
                borderRadius: '70% 30% 30% 70% / 60% 40% 60% 40%',
                animation: 'float0 9s ease-in-out infinite, glowPulse 5s ease-in-out infinite, colorWave 10s linear infinite',
                boxShadow: '0 15px 35px rgba(59, 130, 246, 0.4)'
            }}></div>

            {/* Main content container */}
            <div style={{
                position: 'relative',
                zIndex: 2,
                maxWidth: '800px',
                margin: '0 auto'
            }}>
                {/* Modern profile avatar */}
                <div style={{
                    width: '180px',
                    height: '180px',
                    borderRadius: '50%',
                    background: `
                        linear-gradient(135deg, 
                            #f59e0b 0%, 
                            #ef4444 20%, 
                            #ec4899 40%, 
                            #8b5cf6 60%, 
                            #6366f1 80%, 
                            #06b6d4 100%)
                    `,
                    margin: '0 auto 3rem',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: '5rem',
                    color: 'white',
                    boxShadow: `
                        0 25px 50px rgba(245, 158, 11, 0.3),
                        0 0 40px rgba(236, 72, 153, 0.4),
                        0 0 80px rgba(139, 92, 246, 0.2),
                        0 0 0 1px rgba(255, 255, 255, 0.2),
                        inset 0 1px 0 rgba(255, 255, 255, 0.3)
                    `,
                    animation: 'morphGlow 4s ease-in-out infinite, gradientShift 6s ease-in-out infinite',
                    position: 'relative',
                    overflow: 'hidden',
                    backgroundSize: '200% 200%'
                }}>
                    {/* Shine effect */}
                    <div style={{
                        position: 'absolute',
                        top: '-50%',
                        left: '-50%',
                        width: '200%',
                        height: '200%',
                        background: 'linear-gradient(45deg, transparent, rgba(255,255,255,0.3), transparent)',
                        animation: 'shimmer 2s linear infinite'
                    }}></div>

                    {/* Avatar Icon */}
                    <a href="/images/518735148_1155580089944612_200122795366869623_n (1).jpg">
                        <img src="/images/518735148_1155580089944612_200122795366869623_n (1).jpg" alt="Avatar" style={{
                            width: '100%',
                            height: '100%',
                            objectFit: 'cover',
                            borderRadius: '50%'
                        }} />
                    </a>

                    {/* Gradient overlay for better contrast */}
                    <div style={{
                        position: 'absolute',
                        top: '0',
                        left: '0',
                        right: '0',
                        bottom: '0',
                        borderRadius: '50%',
                        background: 'linear-gradient(135deg, rgba(182, 201, 155, 0.1) 0%, rgba(152, 167, 124, 0.1) 100%)',
                        zIndex: 1,
                        pointerEvents: 'none'
                    }}></div>
                </div>

                <h1 style={{
                    fontSize: '4.5rem',
                    fontWeight: '800',
                    marginBottom: '1.5rem',
                    background: `linear-gradient(135deg, 
                        #f59e0b 0%, 
                        #ef4444 15%, 
                        #ec4899 30%, 
                        #8b5cf6 45%, 
                        #6366f1 60%, 
                        #06b6d4 75%, 
                        #10b981 90%, 
                        #f59e0b 100%)`,
                    WebkitBackgroundClip: 'text',
                    WebkitTextFillColor: 'transparent',
                    backgroundClip: 'text',
                    backgroundSize: '200% 200%',
                    animation: 'slideInRight 0.8s ease-out, gradientShift 8s ease-in-out infinite',
                    letterSpacing: '-0.02em',
                    lineHeight: '1.1'
                }}>
                    Ngô Thanh Tân
                </h1>

                <div style={{
                    fontSize: '1.8rem',
                    color: '#475569',
                    marginBottom: '2rem',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    animation: 'slideInLeft 0.8s ease-out 0.2s both'
                }}>
                    <span style={{
                        background: 'linear-gradient(135deg, #f59e0b 0%, #ef4444 25%, #ec4899 50%, #8b5cf6 75%, #06b6d4 100%)',
                        WebkitBackgroundClip: 'text',
                        WebkitTextFillColor: 'transparent',
                        backgroundClip: 'text',
                        fontWeight: '700',
                        backgroundSize: '200% 200%',
                        animation: 'gradientShift 6s ease-in-out infinite',
                        letterSpacing: '-0.01em'
                    }}>
                        Full-Stack Developer
                    </span>
                </div>                {/* Stats bar */}
                <div style={{
                    display: 'flex',
                    justifyContent: 'center',
                    gap: '2rem',
                    marginBottom: '2rem',
                    animation: 'fadeInUp 0.8s ease-out 0.4s both'
                }}>
                    {[
                        { number: '2+', label: 'Năm Kinh Nghiệm' },
                        { number: '5+', label: 'Dự Án Hoàn Thành' },
                        { number: '100%', label: 'Cam Kết Chất Lượng' }
                    ].map((stat, index) => (
                        <div key={index} style={{
                            textAlign: 'center',
                            padding: '1rem 1.5rem',
                            background: 'rgba(255,255,255,0.8)',
                            borderRadius: '15px',
                            backdropFilter: 'blur(10px)',
                            boxShadow: '0 5px 15px rgba(0,0,0,0.1)',
                            minWidth: '120px'
                        }}>
                            <div style={{
                                fontSize: '1.8rem',
                                fontWeight: 'bold',
                                color: '#B6C99B',
                                marginBottom: '0.5rem'
                            }}>
                                {stat.number}
                            </div>
                            <div style={{
                                fontSize: '0.9rem',
                                color: '#666',
                                fontWeight: '500'
                            }}>
                                {stat.label}
                            </div>
                        </div>
                    ))}
                </div>

                <p style={{
                    fontSize: '1.2rem',
                    color: '#666',
                    marginBottom: '3rem',
                    maxWidth: '600px',
                    lineHeight: '1.8',
                    margin: '0 auto 3rem',
                    animation: 'fadeInUp 0.8s ease-out 0.6s both'
                }}>
                    🎯 Sinh năm 2005, đam mê công nghệ và phát triển phần mềm.
                    <br />
                    💡 Chuyên về Django, React, và các công nghệ web hiện đại.
                    <br />
                    ⚡ Luôn sẵn sàng học hỏi và đối mặt với thử thách mới!
                </p>

                <div style={{
                    display: 'flex',
                    gap: '1rem',
                    justifyContent: 'center',
                    flexWrap: 'wrap',
                    animation: 'fadeInUp 0.8s ease-out 0.8s both'
                }}>
                    <a
                        href="#section3"
                        style={{
                            padding: '15px 30px',
                            background: 'linear-gradient(135deg, #B6C99B 0%, #98A77C 100%)',
                            color: 'white',
                            textDecoration: 'none',
                            borderRadius: '50px',
                            fontWeight: '600',
                            transition: 'all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)',
                            boxShadow: '0 5px 15px rgba(182, 201, 155, 0.4)',
                            display: 'inline-flex',
                            alignItems: 'center',
                            gap: '0.5rem'
                        }}
                        onMouseEnter={(e) => {
                            const target = e.target as HTMLElement;
                            target.style.transform = 'translateY(-3px) scale(1.05)'
                            target.style.boxShadow = '0 8px 25px rgba(182, 201, 155, 0.5)'
                        }}
                        onMouseLeave={(e) => {
                            const target = e.target as HTMLElement;
                            target.style.transform = 'translateY(0) scale(1)'
                            target.style.boxShadow = '0 5px 15px rgba(182, 201, 155, 0.4)'
                        }}
                    >
                        🚀 Xem Dự Án
                    </a>
                    <a
                        href="#section4"
                        style={{
                            padding: '15px 30px',
                            background: 'rgba(255,255,255,0.9)',
                            color: '#B6C99B',
                            textDecoration: 'none',
                            borderRadius: '50px',
                            fontWeight: '600',
                            border: '2px solid #B6C99B',
                            transition: 'all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)',
                            backdropFilter: 'blur(10px)',
                            display: 'inline-flex',
                            alignItems: 'center',
                            gap: '0.5rem'
                        }}
                        onMouseEnter={(e) => {
                            const target = e.target as HTMLElement;
                            target.style.background = '#B6C99B'
                            target.style.color = 'white'
                            target.style.transform = 'translateY(-3px)'
                        }}
                        onMouseLeave={(e) => {
                            const target = e.target as HTMLElement;
                            target.style.background = 'rgba(255,255,255,0.9)'
                            target.style.color = '#B6C99B'
                            target.style.transform = 'translateY(0)'
                        }}
                    >
                        💬 Liên Hệ
                    </a>
                </div>
            </div>
        </section>
    )
}

const SimpleSection = ({ id, title, children, bg = 'white' }) => (
    <section id={id} style={{
        padding: '100px 20px',
        background: bg,
        minHeight: '50vh',
        position: 'relative',
        overflow: 'hidden'
    }}>
        {/* Floating decorative elements */}
        <div style={{
            position: 'absolute',
            top: '10%',
            left: '5%',
            width: '60px',
            height: '60px',
            background: 'linear-gradient(135deg, rgba(245, 158, 11, 0.3) 0%, rgba(236, 72, 153, 0.3) 100%)',
            borderRadius: '50%',
            animation: 'float0 8s ease-in-out infinite, colorWave 6s linear infinite',
            boxShadow: '0 10px 25px rgba(245, 158, 11, 0.2)'
        }}></div>
        <div style={{
            position: 'absolute',
            top: '20%',
            right: '8%',
            width: '80px',
            height: '80px',
            background: 'linear-gradient(135deg, rgba(139, 92, 246, 0.3) 0%, rgba(59, 130, 246, 0.3) 100%)',
            borderRadius: '30% 70% 70% 30%',
            animation: 'float1 10s ease-in-out infinite, morphGlow 8s ease-in-out infinite',
            boxShadow: '0 8px 20px rgba(139, 92, 246, 0.3)'
        }}></div>
        <div style={{
            position: 'absolute',
            bottom: '15%',
            left: '12%',
            width: '40px',
            height: '40px',
            background: 'linear-gradient(135deg, rgba(16, 185, 129, 0.4) 0%, rgba(245, 158, 11, 0.3) 100%)',
            borderRadius: '60% 40% 40% 60%',
            animation: 'float2 7s ease-in-out infinite, glowPulse 5s ease-in-out infinite',
            boxShadow: '0 6px 15px rgba(16, 185, 129, 0.3)'
        }}></div>

        <div style={{ maxWidth: '1200px', margin: '0 auto', position: 'relative', zIndex: 2 }}>
            <h2 style={{
                fontSize: '2.5rem',
                fontWeight: 'bold',
                textAlign: 'center',
                marginBottom: '3rem',
                background: 'linear-gradient(135deg, #f59e0b 0%, #ef4444 25%, #ec4899 50%, #8b5cf6 75%, #06b6d4 100%)',
                WebkitBackgroundClip: 'text',
                WebkitTextFillColor: 'transparent',
                backgroundClip: 'text',
                backgroundSize: '200% 200%',
                animation: 'gradientShift 4s ease-in-out infinite'
            }}>
                {title}
            </h2>
            {children}
        </div>
    </section>
)

function SimpleApp() {
    useEffect(() => {
        document.documentElement.style.scrollBehavior = 'smooth'
    }, [])

    return (
        <div style={{ fontFamily: 'Poppins, sans-serif' }}>
            <SimpleNavbar />
            <SimpleHero />

            <SimpleSection id="section1" title="Giới Thiệu" bg="linear-gradient(135deg, #e8f5e8 0%, #f3e5f5 30%, #e3f2fd 60%, #fff3e0 100%)">
                <div className="about-grid" style={{
                    display: 'grid',
                    gridTemplateColumns: '1fr 2fr',
                    gap: '4rem',
                    alignItems: 'center',
                    maxWidth: '1000px',
                    margin: '0 auto'
                }}>
                    {/* Left side - Image placeholder */}
                    <div className="about-image" style={{
                        position: 'relative',
                        display: 'flex',
                        justifyContent: 'center'
                    }}>
                        {/* Main Profile Card with Real Photo */}
                        <div style={{
                            width: '280px',
                            height: '350px',
                            borderRadius: '25px',
                            position: 'relative',
                            overflow: 'hidden',
                            boxShadow: '0 25px 50px rgba(0,0,0,0.15)',
                            background: 'linear-gradient(135deg, #B6C99B 0%, #98A77C 100%)',
                            padding: '8px',
                            transform: 'perspective(1000px) rotateY(-5deg)',
                            transition: 'all 0.5s cubic-bezier(0.23, 1, 0.320, 1)',
                            cursor: 'pointer'
                        }}
                            onMouseEnter={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.transform = 'perspective(1000px) rotateY(0deg) scale(1.05)'
                                target.style.boxShadow = '0 35px 70px rgba(0,0,0,0.25)'
                            }}
                            onMouseLeave={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.transform = 'perspective(1000px) rotateY(-5deg) scale(1)'
                                target.style.boxShadow = '0 25px 50px rgba(0,0,0,0.15)'
                            }}>
                            {/* Profile Image */}
                            <div style={{
                                width: '100%',
                                height: '100%',
                                borderRadius: '18px',
                                background: `
                                    linear-gradient(135deg, 
                                        rgba(182, 201, 155, 0.1) 0%, 
                                        rgba(152, 167, 124, 0.2) 100%
                                    ),
                                    radial-gradient(circle at 30% 20%, rgba(182, 201, 155, 0.3), transparent 50%),
                                    radial-gradient(circle at 70% 80%, rgba(152, 167, 124, 0.2), transparent 50%)
                                `,
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '4rem',
                                color: '#B6C99B',
                                fontWeight: 'bold',
                                textShadow: '0 2px 10px rgba(182, 201, 155, 0.3)',
                                backgroundSize: 'cover',
                                backgroundPosition: 'center',
                                backgroundRepeat: 'no-repeat',
                                position: 'relative',
                                overflow: 'hidden'
                            }}>
                                {/* Profile Icon */}
                                <div style={{
                                    fontSize: '4rem',
                                    marginBottom: '1rem',
                                    position: 'relative',
                                    zIndex: 2
                                }}>
                                    👨‍💻
                                </div>

                                {/* Decorative patterns */}
                                <div style={{
                                    position: 'absolute',
                                    top: '20px',
                                    right: '20px',
                                    width: '30px',
                                    height: '30px',
                                    background: 'rgba(182, 201, 155, 0.3)',
                                    borderRadius: '50%',
                                    animation: 'pulse 2s ease-in-out infinite'
                                }}></div>
                                <div style={{
                                    position: 'absolute',
                                    bottom: '30px',
                                    left: '30px',
                                    width: '20px',
                                    height: '20px',
                                    background: 'rgba(152, 167, 124, 0.4)',
                                    borderRadius: '50%',
                                    animation: 'pulse 2s ease-in-out infinite 0.5s'
                                }}></div>

                                {/* Overlay gradient */}
                                <div style={{
                                    position: 'absolute',
                                    bottom: '0',
                                    left: '0',
                                    right: '0',
                                    height: '40%',
                                    background: 'linear-gradient(transparent, rgba(0,0,0,0.4))',
                                    display: 'flex',
                                    alignItems: 'flex-end',
                                    padding: '1.5rem',
                                    color: 'white'
                                }}>
                                    <div>
                                        <div style={{ fontSize: '1.2rem', fontWeight: 'bold', marginBottom: '0.5rem', textShadow: '0 2px 4px rgba(0,0,0,0.5)' }}>
                                            Ngô Thanh Tân
                                        </div>
                                        <div style={{ fontSize: '0.9rem', opacity: '0.9', textShadow: '0 1px 2px rgba(0,0,0,0.5)' }}>
                                            Full-Stack Developer
                                        </div>
                                    </div>
                                </div>

                                {/* Shine effect on hover */}
                                <div style={{
                                    position: 'absolute',
                                    top: '-50%',
                                    left: '-50%',
                                    width: '200%',
                                    height: '200%',
                                    background: 'linear-gradient(45deg, transparent, rgba(255,255,255,0.1), transparent)',
                                    transform: 'rotate(45deg)',
                                    transition: 'transform 0.6s ease',
                                    opacity: '0',
                                    pointerEvents: 'none'
                                }}
                                    className="shine-effect"></div>
                            </div>

                            {/* Floating particles */}
                            {[...Array(8)].map((_, i) => (
                                <div key={i} style={{
                                    position: 'absolute',
                                    width: `${Math.random() * 4 + 2}px`,
                                    height: `${Math.random() * 4 + 2}px`,
                                    background: `rgba(255,255,255,${Math.random() * 0.6 + 0.2})`,
                                    borderRadius: '50%',
                                    top: `${Math.random() * 90 + 5}%`,
                                    left: `${Math.random() * 90 + 5}%`,
                                    animation: `float${i % 3} ${2 + Math.random() * 3}s ease-in-out infinite`,
                                    animationDelay: `${Math.random() * 2}s`
                                }}></div>
                            ))}
                        </div>

                        {/* Enhanced Floating stats with animations */}
                        <div
                            className="floating-stat"
                            style={{
                                position: 'absolute',
                                top: '20px',
                                right: '-20px',
                                background: 'linear-gradient(135deg, white 0%, #f8f9fa 100%)',
                                padding: '1.2rem 1.8rem',
                                borderRadius: '20px',
                                boxShadow: '0 15px 35px rgba(182, 201, 155, 0.3)',
                                border: '3px solid #B6C99B',
                                cursor: 'pointer',
                                transition: 'all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)',
                                overflow: 'hidden'
                            }}
                            onMouseEnter={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.transform = 'translateY(-5px) scale(1.1)'
                                target.style.boxShadow = '0 20px 40px rgba(182, 201, 155, 0.4)'
                            }}
                            onMouseLeave={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.transform = 'translateY(0) scale(1)'
                                target.style.boxShadow = '0 15px 35px rgba(182, 201, 155, 0.3)'
                            }}
                        >
                            {/* Animated background sparkle */}
                            <div style={{
                                position: 'absolute',
                                top: '5px',
                                right: '5px',
                                width: '10px',
                                height: '10px',
                                background: '#B6C99B',
                                borderRadius: '50%',
                                animation: 'pulse 1.5s ease-in-out infinite'
                            }}></div>

                            <div style={{ color: '#B6C99B', fontSize: '1.8rem', fontWeight: 'bold', textAlign: 'center' }}>2+</div>
                            <div style={{ color: '#666', fontSize: '0.85rem', fontWeight: '600', textAlign: 'center' }}>Năm KN</div>
                        </div>

                        <div
                            className="floating-stat"
                            style={{
                                position: 'absolute',
                                bottom: '30px',
                                left: '-30px',
                                background: 'linear-gradient(135deg, white 0%, #f8f9fa 100%)',
                                padding: '1.2rem 1.8rem',
                                borderRadius: '20px',
                                boxShadow: '0 15px 35px rgba(152, 167, 124, 0.3)',
                                border: '3px solid #98A77C',
                                cursor: 'pointer',
                                transition: 'all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)',
                                overflow: 'hidden'
                            }}
                            onMouseEnter={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.transform = 'translateY(-5px) scale(1.1)'
                                target.style.boxShadow = '0 20px 40px rgba(152, 167, 124, 0.4)'
                            }}
                            onMouseLeave={(e) => {
                                const target = e.target as HTMLElement;
                                target.style.transform = 'translateY(0) scale(1)'
                                target.style.boxShadow = '0 15px 35px rgba(152, 167, 124, 0.3)'
                            }}
                        >
                            {/* Animated background sparkle */}
                            <div style={{
                                position: 'absolute',
                                top: '5px',
                                right: '5px',
                                width: '10px',
                                height: '10px',
                                background: '#98A77C',
                                borderRadius: '50%',
                                animation: 'pulse 1.5s ease-in-out infinite'
                            }}></div>

                            <div style={{ color: '#98A77C', fontSize: '1.8rem', fontWeight: 'bold', textAlign: 'center' }}>15+</div>
                            <div style={{ color: '#666', fontSize: '0.85rem', fontWeight: '600', textAlign: 'center' }}>Dự Án</div>
                        </div>
                    </div>

                    {/* Right side - Content */}
                    <div className="about-content" style={{ textAlign: 'left' }}>
                        <div style={{
                            fontSize: '1.2rem',
                            background: 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)',
                            WebkitBackgroundClip: 'text',
                            WebkitTextFillColor: 'transparent',
                            fontWeight: '700',
                            marginBottom: '1rem',
                            textTransform: 'uppercase',
                            letterSpacing: '2px',
                            animation: 'gradientShift 4s ease-in-out infinite'
                        }}>
                            Xin chào! Tôi là
                        </div>

                        <h3 style={{
                            fontSize: '3rem',
                            fontWeight: '800',
                            background: 'linear-gradient(135deg, #1f2937 0%, #6366f1 50%, #8b5cf6 100%)',
                            WebkitBackgroundClip: 'text',
                            WebkitTextFillColor: 'transparent',
                            marginBottom: '1.5rem',
                            lineHeight: '1.2',
                            letterSpacing: '-0.02em'
                        }}>
                            Ngô Thanh Tân
                        </h3>

                        <div style={{
                            fontSize: '1.2rem',
                            color: '#475569',
                            lineHeight: '1.8',
                            marginBottom: '2rem',
                            fontWeight: '400'
                        }}>
                            <p style={{ marginBottom: '1.5rem' }}>
                                <span style={{
                                    background: 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)',
                                    WebkitBackgroundClip: 'text',
                                    WebkitTextFillColor: 'transparent',
                                    fontWeight: '700',
                                    fontSize: '1.3rem'
                                }}>🎯 Sinh năm 2005</span>, tôi là một lập trình viên đầy đam mê với
                                <strong style={{ color: '#1f2937' }}> hơn 2 năm kinh nghiệm</strong> trong lĩnh vực phát triển phần mềm.
                                Tôi luôn tìm kiếm những thách thức mới và cơ hội để học hỏi.
                            </p>

                            <p style={{ marginBottom: '1.5rem' }}>
                                <span style={{
                                    background: 'linear-gradient(135deg, #8b5cf6 0%, #ec4899 100%)',
                                    WebkitBackgroundClip: 'text',
                                    WebkitTextFillColor: 'transparent',
                                    fontWeight: '700',
                                    fontSize: '1.3rem'
                                }}>💡 Chuyên về</span> <strong style={{ color: '#1f2937' }}>Django Backend Development</strong> và
                                <strong style={{ color: '#1f2937' }}> React Frontend</strong>, tôi đã tham gia phát triển nhiều dự án thực tế
                                từ hệ thống điểm danh thông minh đến các ứng dụng quản lý doanh nghiệp.
                            </p>

                            <p style={{ marginBottom: '1.5rem' }}>
                                🚀 Điểm mạnh của tôi là khả năng <strong>tư duy logic</strong>,
                                <strong> giải quyết vấn đề</strong> và <strong>học hỏi nhanh chóng</strong>.
                                Tôi luôn cập nhật những công nghệ mới nhất và áp dụng best practices
                                trong mọi dự án.
                            </p>

                            <p>
                                🎨 Ngoài coding, tôi còn có khiếu thẩm mỹ tốt trong việc thiết kế UI/UX,
                                giúp tạo ra những sản phẩm không chỉ hoạt động tốt mà còn
                                <strong> đẹp mắt và thân thiện</strong> với người dùng.
                            </p>
                        </div>

                        {/* Key highlights */}
                        <div className="highlight-grid" style={{
                            display: 'grid',
                            gridTemplateColumns: 'repeat(2, 1fr)',
                            gap: '1rem',
                            marginBottom: '2rem'
                        }}>
                            {[
                                { icon: '🏆', title: 'Chất lượng cao', desc: 'Code clean & scalable' },
                                { icon: '⚡', title: 'Hiệu suất tối ưu', desc: 'Performance-focused' },
                                { icon: '🤝', title: 'Làm việc nhóm', desc: 'Collaborative spirit' },
                                { icon: '📚', title: 'Học hỏi liên tục', desc: 'Always learning' }
                            ].map((item, index) => (
                                <div key={index} className="highlight-card" style={{
                                    background: 'linear-gradient(135deg, white 0%, #fafbfc 100%)',
                                    padding: '1.5rem',
                                    borderRadius: '15px',
                                    boxShadow: '0 8px 25px rgba(0,0,0,0.08)',
                                    border: '2px solid transparent',
                                    backgroundClip: 'padding-box',
                                    textAlign: 'center',
                                    position: 'relative',
                                    overflow: 'hidden',
                                    cursor: 'pointer',
                                    transition: 'all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)'
                                }}
                                    onMouseEnter={(e) => {
                                        const target = e.target as HTMLElement;
                                        target.style.transform = 'translateY(-8px) scale(1.02)'
                                        target.style.boxShadow = '0 15px 35px rgba(182, 201, 155, 0.2)'
                                        target.style.borderColor = '#B6C99B'
                                    }}
                                    onMouseLeave={(e) => {
                                        const target = e.target as HTMLElement;
                                        target.style.transform = 'translateY(0) scale(1)'
                                        target.style.boxShadow = '0 8px 25px rgba(0,0,0,0.08)'
                                        target.style.borderColor = 'transparent'
                                    }}>
                                    <div style={{ fontSize: '1.5rem', marginBottom: '0.5rem' }}>{item.icon}</div>
                                    <div style={{ fontWeight: '600', color: '#333', fontSize: '0.9rem', marginBottom: '0.25rem' }}>
                                        {item.title}
                                    </div>
                                    <div style={{ color: '#666', fontSize: '0.8rem' }}>{item.desc}</div>
                                </div>
                            ))}
                        </div>

                        {/* Call to action */}
                        <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap' }}>
                            <a
                                href="#section3"
                                style={{
                                    padding: '14px 28px',
                                    background: 'linear-gradient(135deg, #f59e0b 0%, #ef4444 50%, #ec4899 100%)',
                                    color: 'white',
                                    textDecoration: 'none',
                                    borderRadius: '25px',
                                    fontWeight: '600',
                                    fontSize: '0.95rem',
                                    transition: 'all 0.3s ease',
                                    display: 'inline-flex',
                                    alignItems: 'center',
                                    gap: '0.5rem',
                                    boxShadow: '0 10px 25px rgba(245, 158, 11, 0.3)'
                                }}
                                onMouseEnter={(e) => {
                                    const target = e.target as HTMLElement;
                                    target.style.transform = 'translateY(-2px) scale(1.05)';
                                    target.style.boxShadow = '0 15px 35px rgba(245, 158, 11, 0.4)';
                                }}
                                onMouseLeave={(e) => {
                                    const target = e.target as HTMLElement;
                                    target.style.transform = 'translateY(0) scale(1)';
                                    target.style.boxShadow = '0 10px 25px rgba(245, 158, 11, 0.3)';
                                }}
                            >
                                🚀 Xem Dự Án Của Tôi
                            </a>

                            <a
                                href="#section4"
                                style={{
                                    padding: '12px 24px',
                                    background: 'transparent',
                                    color: '#8b5cf6',
                                    textDecoration: 'none',
                                    borderRadius: '25px',
                                    fontWeight: '600',
                                    fontSize: '0.95rem',
                                    border: '2px solid #8b5cf6',
                                    transition: 'all 0.3s ease',
                                    display: 'inline-flex',
                                    alignItems: 'center',
                                    gap: '0.5rem'
                                }}
                                onMouseEnter={(e) => {
                                    const target = e.target as HTMLElement;
                                    target.style.background = 'linear-gradient(135deg, #8b5cf6 0%, #6366f1 100%)';
                                    target.style.color = 'white';
                                    target.style.transform = 'translateY(-2px) scale(1.05)';
                                    target.style.boxShadow = '0 10px 25px rgba(139, 92, 246, 0.4)';
                                }}
                                onMouseLeave={(e) => {
                                    const target = e.target as HTMLElement;
                                    target.style.background = 'transparent';
                                    target.style.color = '#8b5cf6';
                                    target.style.transform = 'translateY(0) scale(1)';
                                    target.style.boxShadow = 'none';
                                }}
                            >
                                💬 Liên Hệ Ngay
                            </a>
                        </div>
                    </div>
                </div>

                {/* Mobile responsive version */}
                <style jsx>{`
                    @media (max-width: 768px) {
                        .about-grid {
                            grid-template-columns: 1fr !important;
                            gap: 2rem !important;
                            text-align: center !important;
                        }
                        .about-content {
                            text-align: center !important;
                        }
                        .highlight-grid {
                            grid-template-columns: 1fr !important;
                        }
                    }
                `}</style>
            </SimpleSection>

            <SimpleSection id="section2" title="Kỹ Năng" bg="linear-gradient(135deg, #ffecd2 0%, #fcb69f 30%, #ffeaa7 60%, #fab1a0 100%)">
                <div className="skills-grid" style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(4, 1fr)',
                    gap: '2rem',
                    maxWidth: '1200px',
                    margin: '0 auto'
                }}>
                    {[
                        {
                            title: '🎨 Frontend Development',
                            skills: ['React', 'TypeScript', 'JavaScript', 'HTML5', 'CSS3', 'Tailwind CSS', 'Bootstrap'],
                            color: '#e3f2fd',
                            icon: '💻'
                        },
                        {
                            title: '⚙️ Backend Development',
                            skills: ['Django', 'Django REST', 'Python', 'ASP .NET', 'JWT Authentication', 'API Development'],
                            color: '#f3e5f5',
                            icon: '🔧'
                        },
                        {
                            title: '🗄️ Database & Storage',
                            skills: ['MySQL', 'PostgreSQL', 'SQLite', 'Database Design'],
                            color: '#fff3e0',
                            icon: '📊'
                        },
                        {
                            title: '🛠️ Tools & Technologies',
                            skills: ['Git', 'GitHub', 'VS Code', 'Docker', 'Postman', 'OpenCV'],
                            color: '#e8f5e8',
                            icon: '⚡'
                        }
                    ].map((category, index) => (
                        <div key={index} style={{
                            background: 'rgba(255, 255, 255, 0.95)',
                            padding: '2.5rem 2rem',
                            borderRadius: '25px',
                            boxShadow: '0 15px 40px rgba(0,0,0,0.15)',
                            textAlign: 'center',
                            border: '1px solid rgba(255, 255, 255, 0.3)',
                            backdropFilter: 'blur(10px)',
                            transition: 'all 0.3s ease',
                            position: 'relative',
                            overflow: 'hidden'
                        }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.transform = 'translateY(-10px) scale(1.02)'
                                e.currentTarget.style.boxShadow = '0 25px 50px rgba(0,0,0,0.2)'
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.transform = 'translateY(0) scale(1)'
                                e.currentTarget.style.boxShadow = '0 15px 40px rgba(0,0,0,0.15)'
                            }}
                        >
                            {/* Background decoration */}
                            <div style={{
                                position: 'absolute',
                                top: '-50px',
                                right: '-50px',
                                width: '100px',
                                height: '100px',
                                background: category.color,
                                borderRadius: '50%',
                                opacity: 0.3,
                                zIndex: 0
                            }}></div>

                            {/* Category icon and title */}
                            <div style={{
                                fontSize: '3rem',
                                marginBottom: '1rem',
                                position: 'relative',
                                zIndex: 1
                            }}>
                                {category.icon}
                            </div>
                            <h3 style={{
                                marginBottom: '1.5rem',
                                color: '#2d3748',
                                fontSize: '1.3rem',
                                fontWeight: '700',
                                position: 'relative',
                                zIndex: 1
                            }}>
                                {category.title}
                            </h3>

                            {/* Skills grid */}
                            <div style={{
                                display: 'grid',
                                gridTemplateColumns: 'repeat(auto-fit, minmax(120px, 1fr))',
                                gap: '0.8rem',
                                justifyItems: 'center',
                                position: 'relative',
                                zIndex: 1
                            }}>
                                {category.skills.map((skill, skillIndex) => (
                                    <div key={skillIndex} style={{
                                        background: 'linear-gradient(135deg, rgba(99, 102, 241, 0.1) 0%, rgba(139, 92, 246, 0.2) 100%)',
                                        color: '#374151',
                                        padding: '0.7rem 1rem',
                                        borderRadius: '20px',
                                        fontSize: '0.85rem',
                                        fontWeight: '600',
                                        border: '2px solid rgba(99, 102, 241, 0.3)',
                                        transition: 'all 0.3s ease',
                                        cursor: 'pointer',
                                        minWidth: '100px',
                                        textAlign: 'center'
                                    }}
                                        onMouseEnter={(e) => {
                                            const target = e.target as HTMLElement;
                                            target.style.background = 'linear-gradient(135deg, #6366f1 0%, #8b5cf6 100%)'
                                            target.style.color = 'white'
                                            target.style.transform = 'translateY(-3px) scale(1.05)'
                                            target.style.boxShadow = '0 8px 20px rgba(99, 102, 241, 0.4)'
                                        }}
                                        onMouseLeave={(e) => {
                                            const target = e.target as HTMLElement;
                                            target.style.background = 'linear-gradient(135deg, rgba(99, 102, 241, 0.1) 0%, rgba(139, 92, 246, 0.2) 100%)'
                                            target.style.color = '#374151'
                                            target.style.transform = 'translateY(0) scale(1)'
                                            target.style.boxShadow = 'none'
                                        }}
                                    >
                                        {skill}
                                    </div>
                                ))}
                            </div>
                        </div>
                    ))}
                </div>

                {/* Skills Responsive CSS */}
                <style jsx>{`
                    @media (max-width: 1024px) {
                        .skills-grid {
                            grid-template-columns: repeat(2, 1fr) !important;
                            gap: 1.5rem !important;
                        }
                    }
                    @media (max-width: 768px) {
                        .skills-grid {
                            grid-template-columns: 1fr !important;
                            gap: 1.5rem !important;
                        }
                    }
                `}</style>
            </SimpleSection>

            <SimpleSection id="section3" title="Dự Án" bg="linear-gradient(135deg, #fef3f2 0%, #fef5f3 50%, #f0f4ff 100%)">
                <div style={{
                    display: 'grid',
                    gridTemplateColumns: 'repeat(auto-fit, minmax(380px, 1fr))',
                    gap: '2.5rem',
                    maxWidth: '1200px',
                    margin: '0 auto'
                }}>
                    {[
                        {
                            title: 'Face Attendance System',
                            desc: 'Hệ thống điểm danh thông minh sử dụng nhận diện khuôn mặt với OpenCV và Django. Tích hợp machine learning để tự động nhận diện và ghi nhận thời gian có mặt.',
                            tech: ['Django', 'OpenCV', 'Python', 'Face Recognition', 'MySQL'],
                            github: 'https://github.com/Face-Attendances/Face-Attendance',
                            icon: '🔍',
                            color: 'linear-gradient(135deg, #f59e0b 0%, #ef4444 50%, #ec4899 100%)'
                        },
                        {
                            title: 'Dental Management System',
                            desc: 'Hệ thống quản lý phòng khám nha khoa toàn diện với Django REST API. Quản lý bệnh nhân, lịch hẹn, hồ sơ điều trị và báo cáo thống kê.',
                            tech: ['Django REST', 'JWT', 'MySQL', 'API Development', 'Bootstrap'],
                            github: 'https://github.com/fourgay/dental-system',
                            icon: '🦷',
                            color: 'linear-gradient(135deg, #8b5cf6 0%, #6366f1 50%, #06b6d4 100%)'
                        },
                        {
                            title: 'Fruit Management System',
                            desc: 'Ứng dụng web quản lý cửa hàng trái cây với dashboard thống kê, quản lý kho, đơn hàng và khách hàng. Giao diện responsive và thân thiện.',
                            tech: ['Django', 'MySQL', 'Bootstrap', 'Chart.js', 'JavaScript'],
                            github: 'https://github.com/Fruits-manage/QLDA',
                            icon: '🍎',
                            color: 'linear-gradient(135deg, #10b981 0%, #f59e0b 50%, #ef4444 100%)'
                        }
                    ].map((project, index) => (
                        <div key={index} style={{
                            background: 'white',
                            borderRadius: '25px',
                            overflow: 'hidden',
                            boxShadow: '0 15px 35px rgba(0,0,0,0.1)',
                            transition: 'all 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275)',
                            border: '1px solid #f0f0f0',
                            position: 'relative'
                        }}
                            onMouseEnter={(e) => {
                                const target = e.currentTarget as HTMLElement;
                                target.style.transform = 'translateY(-15px) scale(1.02)'
                                target.style.boxShadow = '0 25px 50px rgba(0,0,0,0.15)'
                            }}
                            onMouseLeave={(e) => {
                                const target = e.currentTarget as HTMLElement;
                                target.style.transform = 'translateY(0) scale(1)'
                                target.style.boxShadow = '0 15px 35px rgba(0,0,0,0.1)'
                            }}
                        >
                            {/* Project Header with Gradient */}
                            <div style={{
                                height: '220px',
                                background: project.color,
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '4rem',
                                position: 'relative',
                                overflow: 'hidden'
                            }}>
                                {/* Decorative Elements */}
                                <div style={{
                                    position: 'absolute',
                                    top: '20px',
                                    right: '20px',
                                    background: 'rgba(255,255,255,0.2)',
                                    borderRadius: '50%',
                                    width: '60px',
                                    height: '60px',
                                    animation: 'float0 3s ease-in-out infinite'
                                }}></div>
                                <div style={{
                                    position: 'absolute',
                                    bottom: '20px',
                                    left: '20px',
                                    background: 'rgba(255,255,255,0.15)',
                                    borderRadius: '50%',
                                    width: '40px',
                                    height: '40px',
                                    animation: 'float1 4s ease-in-out infinite'
                                }}></div>

                                {/* Main Icon */}
                                <div style={{
                                    fontSize: '4rem',
                                    textShadow: '0 5px 15px rgba(0,0,0,0.2)',
                                    animation: 'float2 2s ease-in-out infinite'
                                }}>
                                    {project.icon}
                                </div>
                            </div>

                            {/* Project Content */}
                            <div style={{ padding: '2.5rem' }}>
                                <h3 style={{
                                    marginBottom: '1rem',
                                    color: '#333',
                                    fontSize: '1.4rem',
                                    fontWeight: 'bold'
                                }}>
                                    {project.title}
                                </h3>

                                <p style={{
                                    color: '#666',
                                    marginBottom: '2rem',
                                    lineHeight: '1.7',
                                    fontSize: '0.95rem'
                                }}>
                                    {project.desc}
                                </p>

                                {/* Technology Tags */}
                                <div style={{
                                    display: 'flex',
                                    flexWrap: 'wrap',
                                    gap: '0.6rem',
                                    marginBottom: '2rem'
                                }}>
                                    {project.tech.map((tech, techIndex) => (
                                        <span key={techIndex} style={{
                                            background: 'linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%)',
                                            color: '#495057',
                                            padding: '0.4rem 0.9rem',
                                            borderRadius: '20px',
                                            fontSize: '0.8rem',
                                            fontWeight: '600',
                                            border: '1px solid #dee2e6',
                                            transition: 'all 0.3s ease'
                                        }}>
                                            {tech}
                                        </span>
                                    ))}
                                </div>

                                {/* GitHub Link Button */}
                                <a
                                    href={project.github}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    style={{
                                        display: 'inline-flex',
                                        alignItems: 'center',
                                        gap: '0.5rem',
                                        padding: '12px 24px',
                                        background: 'linear-gradient(135deg, #24292e 0%, #1a1e22 100%)',
                                        color: 'white',
                                        textDecoration: 'none',
                                        borderRadius: '25px',
                                        fontWeight: '600',
                                        fontSize: '0.9rem',
                                        transition: 'all 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275)',
                                        boxShadow: '0 5px 15px rgba(36, 41, 46, 0.3)'
                                    }}
                                    onMouseEnter={(e) => {
                                        const target = e.target as HTMLElement;
                                        target.style.transform = 'translateY(-2px) scale(1.05)'
                                        target.style.boxShadow = '0 8px 25px rgba(36, 41, 46, 0.4)'
                                    }}
                                    onMouseLeave={(e) => {
                                        const target = e.target as HTMLElement;
                                        target.style.transform = 'translateY(0) scale(1)'
                                        target.style.boxShadow = '0 5px 15px rgba(36, 41, 46, 0.3)'
                                    }}
                                >
                                    <svg
                                        width="20"
                                        height="20"
                                        fill="currentColor"
                                        viewBox="0 0 24 24"
                                    >
                                        <path d="M12 0C5.37 0 0 5.37 0 12c0 5.31 3.435 9.795 8.205 11.385.6.105.825-.255.825-.57 0-.285-.015-1.23-.015-2.235-3.015.555-3.795-.735-4.035-1.41-.135-.345-.72-1.41-1.23-1.695-.42-.225-1.02-.78-.015-.795.945-.015 1.62.87 1.845 1.23 1.08 1.815 2.805 1.305 3.495.99.105-.78.42-1.305.765-1.605-2.67-.3-5.46-1.335-5.46-5.925 0-1.305.465-2.385 1.23-3.225-.12-.3-.54-1.53.12-3.18 0 0 1.005-.315 3.3 1.23.96-.27 1.98-.405 3-.405s2.04.135 3 .405c2.295-1.56 3.3-1.23 3.3-1.23.66 1.65.24 2.88.12 3.18.765.84 1.23 1.905 1.23 3.225 0 4.605-2.805 5.625-5.475 5.925.435.375.81 1.095.81 2.22 0 1.605-.015 2.895-.015 3.3 0 .315.225.69.825.57A12.02 12.02 0 0024 12c0-6.63-5.37-12-12-12z" />
                                    </svg>
                                    Xem trên GitHub
                                </a>
                            </div>
                        </div>
                    ))}
                </div>
            </SimpleSection>

            <SimpleSection id="section4" title="Liên Hệ" bg="linear-gradient(135deg, #ffeef8 0%, #f3e8ff 30%, #e0f2fe 60%, #f1f8e9 100%)">
                <div style={{
                    display: 'grid',
                    gridTemplateColumns: '1fr 1fr',
                    gap: '4rem',
                    alignItems: 'start'
                }}>
                    <div>
                        <h3 style={{ marginBottom: '2rem', color: '#333' }}>Hãy liên hệ với tôi!</h3>
                        <p style={{ marginBottom: '2rem', color: '#666', lineHeight: '1.8' }}>
                            Tôi luôn sẵn sàng lắng nghe những ý tưởng mới và cơ hội hợp tác thú vị.
                        </p>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                            {[
                                { icon: '📧', title: 'Email', content: 'ngothantan2005@example.com' },
                                { icon: '📱', title: 'Điện Thoại', content: '+84 123 456 789' },
                                { icon: '📍', title: 'Địa Chỉ', content: 'Việt Nam' },
                                { icon: '💻', title: 'GitHub', content: 'github.com/thanhtanlego' }
                            ].map((item, index) => (
                                <div key={index} style={{
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: '1rem',
                                    padding: '1rem',
                                    background: 'white',
                                    borderRadius: '10px',
                                    boxShadow: '0 5px 15px rgba(0,0,0,0.1)'
                                }}>
                                    <span style={{ fontSize: '1.5rem' }}>{item.icon}</span>
                                    <div>
                                        <h4 style={{ marginBottom: '0.25rem', color: '#333' }}>{item.title}</h4>
                                        <p style={{ color: '#666', margin: 0 }}>{item.content}</p>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>

                    <form style={{
                        background: 'white',
                        padding: '2rem',
                        borderRadius: '20px',
                        boxShadow: '0 10px 30px rgba(0,0,0,0.1)'
                    }}>
                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Tên của bạn</label>
                            <input type="text" style={{
                                width: '100%',
                                padding: '12px 16px',
                                border: '2px solid #e9ecef',
                                borderRadius: '10px',
                                fontSize: '1rem'
                            }} />
                        </div>
                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Email</label>
                            <input type="email" style={{
                                width: '100%',
                                padding: '12px 16px',
                                border: '2px solid #e9ecef',
                                borderRadius: '10px',
                                fontSize: '1rem'
                            }} />
                        </div>
                        <div style={{ marginBottom: '1.5rem' }}>
                            <label style={{ display: 'block', marginBottom: '0.5rem', fontWeight: '500' }}>Tin nhắn</label>
                            <textarea style={{
                                width: '100%',
                                padding: '12px 16px',
                                border: '2px solid #e9ecef',
                                borderRadius: '10px',
                                fontSize: '1rem',
                                minHeight: '120px',
                                resize: 'vertical'
                            }}></textarea>
                        </div>
                        <button type="submit" style={{
                            width: '100%',
                            padding: '15px',
                            background: 'linear-gradient(135deg, #B6C99B 0%, #98A77C 100%)',
                            color: 'white',
                            border: 'none',
                            borderRadius: '10px',
                            fontSize: '1.1rem',
                            fontWeight: '600',
                            cursor: 'pointer',
                            transition: 'transform 0.3s ease'
                        }}
                            onMouseEnter={(e) => e.target.style.transform = 'translateY(-2px)'}
                            onMouseLeave={(e) => e.target.style.transform = 'translateY(0)'}
                        >
                            Gửi Tin Nhắn
                        </button>
                    </form>
                </div>
            </SimpleSection>

            {/* Scroll to top button */}
            <button
                onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}
                style={{
                    position: 'fixed',
                    bottom: '30px',
                    right: '30px',
                    width: '60px',
                    height: '60px',
                    borderRadius: '50%',
                    background: 'linear-gradient(135deg, #B6C99B 0%, #98A77C 100%)',
                    border: 'none',
                    color: 'white',
                    fontSize: '1.5rem',
                    cursor: 'pointer',
                    boxShadow: '0 4px 20px rgba(0,0,0,0.3)',
                    transition: 'transform 0.3s ease',
                    zIndex: 1000
                }}
                onMouseEnter={(e) => e.target.style.transform = 'scale(1.1)'}
                onMouseLeave={(e) => e.target.style.transform = 'scale(1)'}
            >
                ↑
            </button>
        </div>
    )
}

export default SimpleApp