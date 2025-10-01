import React from 'react'
import { motion } from 'framer-motion'
import styled from 'styled-components'

const AboutSection = styled.section`
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

const AboutContent = styled.div`
  display: grid;
  grid-template-columns: 1fr 2fr;
  gap: 4rem;
  align-items: center;
  
  @media (max-width: 768px) {
    grid-template-columns: 1fr;
    gap: 2rem;
  }
`

const AboutImage = styled(motion.div)`
  position: relative;
  
  .image-container {
    position: relative;
    border-radius: 20px;
    overflow: hidden;
    background: var(--gradient);
    padding: 4px;
    
    .image-placeholder {
      width: 100%;
      height: 400px;
      background: linear-gradient(45deg, var(--primary-color), var(--secondary-color));
      border-radius: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      font-size: 3rem;
      position: relative;
      
      &::before {
        content: '👨‍💻';
        font-size: 4rem;
      }
    }
  }
  
  &::before {
    content: '';
    position: absolute;
    top: -20px;
    left: -20px;
    right: 20px;
    bottom: 20px;
    background: var(--gradient);
    border-radius: 20px;
    z-index: -1;
    opacity: 0.3;
  }
`

const AboutText = styled(motion.div)`
  h3 {
    font-size: 1.8rem;
    font-weight: 600;
    margin-bottom: 1rem;
    color: var(--text-dark);
  }
  
  p {
    font-size: 1.1rem;
    line-height: 1.8;
    color: var(--text-light);
    margin-bottom: 1.5rem;
  }
`

const StatsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 2rem;
  margin-top: 3rem;
`

const StatItem = styled(motion.div)`
  text-align: center;
  padding: 2rem 1rem;
  background: white;
  border-radius: 15px;
  box-shadow: var(--shadow);
  transition: all 0.3s ease;
  
  &:hover {
    transform: translateY(-5px);
    box-shadow: var(--shadow-hover);
  }
  
  .stat-number {
    font-size: 2.5rem;
    font-weight: 700;
    color: var(--primary-color);
    margin-bottom: 0.5rem;
  }
  
  .stat-label {
    font-size: 1rem;
    color: var(--text-light);
    font-weight: 500;
  }
`

const About: React.FC = () => {
    const stats = [
        { number: "3+", label: "Dự Án" },
        { number: "2+", label: "Năm Kinh Nghiệm" },
        { number: "10+", label: "Công Nghệ" },
        { number: "100%", label: "Cam Kết" }
    ]

    return (
        <AboutSection id="about">
            <Container>
                <SectionTitle
                    initial={{ opacity: 0, y: -50 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.8 }}
                    viewport={{ once: true }}
                >
                    Giới Thiệu
                </SectionTitle>

                <AboutContent>
                    <AboutImage
                        initial={{ opacity: 0, x: -50 }}
                        whileInView={{ opacity: 1, x: 0 }}
                        transition={{ duration: 0.8 }}
                        viewport={{ once: true }}
                    >
                        <div className="image-container">
                            <div className="image-placeholder"></div>
                        </div>
                    </AboutImage>

                    <AboutText
                        initial={{ opacity: 0, x: 50 }}
                        whileInView={{ opacity: 1, x: 0 }}
                        transition={{ duration: 0.8, delay: 0.2 }}
                        viewport={{ once: true }}
                    >
                        <h3>Xin chào! Tôi là Ngô Thanh Tân</h3>
                        <p>
                            Sinh năm 2005, tôi là một lập trình viên đầy đam mê với công nghệ và
                            phát triển phần mềm. Với hơn 2 năm kinh nghiệm trong lĩnh vực lập trình,
                            tôi chuyên về phát triển web với Django và React.
                        </p>
                        <p>
                            Tôi đã tham gia phát triển nhiều dự án thực tế như hệ thống điểm danh
                            khuôn mặt, hệ thống quản lý nha khoa và ứng dụng quản lý trái cây.
                            Mỗi dự án đều mang lại cho tôi kinh nghiệm quý báu và kỹ năng mới.
                        </p>
                        <p>
                            Tôi luôn háo hức học hỏi những công nghệ mới và tìm kiếm cơ hội để
                            áp dụng chúng vào các dự án thực tế, tạo ra những sản phẩm có giá trị
                            và ý nghĩa.
                        </p>
                    </AboutText>
                </AboutContent>

                <StatsGrid>
                    {stats.map((stat, index) => (
                        <StatItem
                            key={index}
                            initial={{ opacity: 0, y: 30 }}
                            whileInView={{ opacity: 1, y: 0 }}
                            transition={{ duration: 0.6, delay: index * 0.1 }}
                            viewport={{ once: true }}
                            whileHover={{ scale: 1.05 }}
                        >
                            <div className="stat-number">{stat.number}</div>
                            <div className="stat-label">{stat.label}</div>
                        </StatItem>
                    ))}
                </StatsGrid>
            </Container>
        </AboutSection>
    )
}

export default About