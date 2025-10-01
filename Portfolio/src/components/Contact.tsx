import React, { useState } from 'react'
import { motion } from 'framer-motion'
import styled from 'styled-components'

const ContactSection = styled.section`
  padding: 100px 0;
  background: var(--bg-light);
`

const Container = styled.div`
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
`

const SectionTitle = styled(motion.h2)`
  font-size: 2.5rem;
  font-weight: 700;
  text-align: center;
  margin-bottom: 60px;
  color: var(--text-dark);
  position: relative;
  
  &::after {
    content: '';
    position: absolute;
    bottom: -10px;
    left: 50%;
    transform: translateX(-50%);
    width: 60px;
    height: 4px;
    background: var(--gradient);
    border-radius: 2px;
  }
`

const ContactContent = styled.div`
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 4rem;
  
  @media (max-width: 768px) {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
`

const ContactInfo = styled(motion.div)`
  h3 {
    font-size: 1.8rem;
    font-weight: 600;
    margin-bottom: 2rem;
    color: var(--text-dark);
  }
  
  p {
    font-size: 1.1rem;
    line-height: 1.8;
    color: var(--text-light);
    margin-bottom: 2rem;
  }
`

const ContactItem = styled(motion.div)`
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: white;
  border-radius: 10px;
  box-shadow: var(--shadow);
  transition: all 0.3s ease;
  
  &:hover {
    transform: translateX(10px);
    box-shadow: var(--shadow-hover);
  }
  
  .icon {
    width: 50px;
    height: 50px;
    background: var(--gradient);
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    color: white;
    font-size: 1.2rem;
  }
  
  .content {
    h4 {
      font-size: 1.1rem;
      font-weight: 600;
      color: var(--text-dark);
      margin-bottom: 0.5rem;
    }
    
    p {
      color: var(--text-light);
      margin: 0;
    }
    
    a {
      color: var(--primary-color);
      text-decoration: none;
      
      &:hover {
        text-decoration: underline;
      }
    }
  }
`

const ContactForm = styled(motion.form)`
  background: white;
  padding: 2rem;
  border-radius: 20px;
  box-shadow: var(--shadow);
`

const FormGroup = styled.div`
  margin-bottom: 1.5rem;
  
  label {
    display: block;
    margin-bottom: 0.5rem;
    font-weight: 500;
    color: var(--text-dark);
  }
  
  input, textarea {
    width: 100%;
    padding: 12px 16px;
    border: 2px solid #e9ecef;
    border-radius: 10px;
    font-size: 1rem;
    transition: all 0.3s ease;
    font-family: inherit;
    
    &:focus {
      outline: none;
      border-color: var(--primary-color);
      box-shadow: 0 0 0 3px rgba(182, 201, 155, 0.1);
    }
  }
  
  textarea {
    resize: vertical;
    min-height: 120px;
  }
`

const SubmitButton = styled(motion.button)`
  width: 100%;
  padding: 15px;
  background: var(--gradient);
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  
  &:hover {
    background: var(--gradient-hover);
    transform: translateY(-2px);
    box-shadow: var(--shadow-hover);
  }
  
  &:disabled {
    opacity: 0.7;
    cursor: not-allowed;
  }
`

const SocialLinks = styled.div`
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
  justify-content: center;
  
  a {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 50px;
    height: 50px;
    background: var(--gradient);
    color: white;
    border-radius: 50%;
    text-decoration: none;
    font-size: 1.2rem;
    transition: all 0.3s ease;
    
    &:hover {
      background: var(--gradient-hover);
      transform: translateY(-3px) scale(1.1);
    }
  }
`

const Contact: React.FC = () => {
    const [formData, setFormData] = useState({
        name: '',
        email: '',
        subject: '',
        message: ''
    })
    const [isSubmitting, setIsSubmitting] = useState(false)

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const { name, value } = e.target
        setFormData(prev => ({
            ...prev,
            [name]: value
        }))
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setIsSubmitting(true)

        // Simulate form submission
        setTimeout(() => {
            alert('Cảm ơn bạn! Tin nhắn đã được gửi thành công!')
            setFormData({ name: '', email: '', subject: '', message: '' })
            setIsSubmitting(false)
        }, 1000)
    }

    const contactItems = [
        {
            icon: "fas fa-envelope",
            title: "Email",
            content: "ngothantan2005@example.com",
            link: "mailto:ngothantan2005@example.com"
        },
        {
            icon: "fas fa-phone",
            title: "Điện Thoại",
            content: "+84 123 456 789",
            link: "tel:+84123456789"
        },
        {
            icon: "fas fa-map-marker-alt",
            title: "Địa Chỉ",
            content: "Việt Nam",
            link: "#"
        },
        {
            icon: "fab fa-github",
            title: "GitHub",
            content: "github.com/thanhtanlego",
            link: "https://github.com/thanhtanlego"
        }
    ]

    return (
        <ContactSection id="contact">
            <Container>
                <SectionTitle
                    initial={{ opacity: 0, y: -50 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.8 }}
                    viewport={{ once: true }}
                >
                    Liên Hệ
                </SectionTitle>

                <ContactContent>
                    <ContactInfo
                        initial={{ opacity: 0, x: -50 }}
                        whileInView={{ opacity: 1, x: 0 }}
                        transition={{ duration: 0.8 }}
                        viewport={{ once: true }}
                    >
                        <h3>Hãy liên hệ với tôi!</h3>
                        <p>
                            Tôi luôn sẵn sàng lắng nghe những ý tưởng mới và cơ hội hợp tác thú vị.
                            Hãy liên hệ với tôi để thảo luận về dự án của bạn!
                        </p>

                        {contactItems.map((item, index) => (
                            <ContactItem
                                key={index}
                                initial={{ opacity: 0, y: 30 }}
                                whileInView={{ opacity: 1, y: 0 }}
                                transition={{ duration: 0.6, delay: index * 0.1 }}
                                viewport={{ once: true }}
                                whileHover={{ scale: 1.02 }}
                            >
                                <div className="icon">
                                    <i className={item.icon}></i>
                                </div>
                                <div className="content">
                                    <h4>{item.title}</h4>
                                    <p>
                                        {item.link !== "#" ? (
                                            <a href={item.link} target="_blank" rel="noopener noreferrer">
                                                {item.content}
                                            </a>
                                        ) : (
                                            item.content
                                        )}
                                    </p>
                                </div>
                            </ContactItem>
                        ))}

                        <SocialLinks>
                            <a href="https://github.com/thanhtanlego" target="_blank" rel="noopener noreferrer">
                                <i className="fab fa-github"></i>
                            </a>
                            <a href="https://linkedin.com/in/ngothantan" target="_blank" rel="noopener noreferrer">
                                <i className="fab fa-linkedin"></i>
                            </a>
                            <a href="https://facebook.com/ngothantan" target="_blank" rel="noopener noreferrer">
                                <i className="fab fa-facebook"></i>
                            </a>
                            <a href="mailto:ngothantan2005@example.com">
                                <i className="fas fa-envelope"></i>
                            </a>
                        </SocialLinks>
                    </ContactInfo>

                    <ContactForm
                        initial={{ opacity: 0, x: 50 }}
                        whileInView={{ opacity: 1, x: 0 }}
                        transition={{ duration: 0.8, delay: 0.2 }}
                        viewport={{ once: true }}
                        onSubmit={handleSubmit}
                    >
                        <FormGroup>
                            <label htmlFor="name">Tên của bạn</label>
                            <input
                                type="text"
                                id="name"
                                name="name"
                                value={formData.name}
                                onChange={handleInputChange}
                                required
                            />
                        </FormGroup>

                        <FormGroup>
                            <label htmlFor="email">Email</label>
                            <input
                                type="email"
                                id="email"
                                name="email"
                                value={formData.email}
                                onChange={handleInputChange}
                                required
                            />
                        </FormGroup>

                        <FormGroup>
                            <label htmlFor="subject">Chủ đề</label>
                            <input
                                type="text"
                                id="subject"
                                name="subject"
                                value={formData.subject}
                                onChange={handleInputChange}
                                required
                            />
                        </FormGroup>

                        <FormGroup>
                            <label htmlFor="message">Tin nhắn</label>
                            <textarea
                                id="message"
                                name="message"
                                value={formData.message}
                                onChange={handleInputChange}
                                required
                            />
                        </FormGroup>

                        <SubmitButton
                            type="submit"
                            disabled={isSubmitting}
                            whileHover={{ scale: 1.02 }}
                            whileTap={{ scale: 0.98 }}
                        >
                            {isSubmitting ? 'Đang gửi...' : 'Gửi Tin Nhắn'}
                        </SubmitButton>
                    </ContactForm>
                </ContactContent>
            </Container>
        </ContactSection>
    )
}

export default Contact