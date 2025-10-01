import React from 'react'
import { motion } from 'framer-motion'
import styled from 'styled-components'

const ProjectsSection = styled.section`
  padding: 100px 0;
  background: white;
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

const ProjectsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 2rem;
`

const ProjectCard = styled(motion.div)`
  background: white;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: var(--shadow);
  transition: all 0.3s ease;
  
  &:hover {
    transform: translateY(-10px);
    box-shadow: var(--shadow-hover);
  }
`

const ProjectImage = styled.div`
  height: 200px;
  background: var(--gradient);
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-size: 3rem;
  
  &::before {
    content: '💻';
    font-size: 4rem;
  }
  
  .project-overlay {
    position: absolute;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.7);
    display: flex;
    align-items: center;
    justify-content: center;
    opacity: 0;
    transition: opacity 0.3s ease;
    
    .overlay-content {
      display: flex;
      gap: 1rem;
      
      a {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 50px;
        height: 50px;
        background: var(--primary-color);
        color: white;
        border-radius: 50%;
        text-decoration: none;
        font-size: 1.2rem;
        transition: all 0.3s ease;
        
        &:hover {
          background: var(--secondary-color);
          transform: scale(1.1);
        }
      }
    }
  }
  
  &:hover .project-overlay {
    opacity: 1;
  }
`

const ProjectContent = styled.div`
  padding: 2rem;
`

const ProjectTitle = styled.h3`
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: var(--text-dark);
`

const ProjectDescription = styled.p`
  color: var(--text-light);
  line-height: 1.6;
  margin-bottom: 1.5rem;
`

const TechStack = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
`

const TechTag = styled.span`
  background: var(--bg-light);
  color: var(--primary-color);
  padding: 4px 12px;
  border-radius: 15px;
  font-size: 0.8rem;
  font-weight: 500;
`

const ProjectLinks = styled.div`
  display: flex;
  gap: 1rem;
  
  a {
    display: inline-flex;
    align-items: center;
    gap: 0.5rem;
    padding: 8px 16px;
    background: var(--gradient);
    color: white;
    text-decoration: none;
    border-radius: 20px;
    font-size: 0.9rem;
    font-weight: 500;
    transition: all 0.3s ease;
    
    &:hover {
      background: var(--gradient-hover);
      transform: translateY(-2px);
    }
    
    &.secondary {
      background: transparent;
      color: var(--primary-color);
      border: 1px solid var(--primary-color);
      
      &:hover {
        background: var(--primary-color);
        color: white;
      }
    }
  }
`

const Projects: React.FC = () => {
  const projects = [
    {
      title: "Face Attendance System",
      description: "Hệ thống điểm danh thông minh sử dụng công nghệ nhận diện khuôn mặt với OpenCV và Django. Cho phép quản lý sinh viên và theo dõi thời gian có mặt một cách tự động và chính xác.",
      techStack: ["Django", "OpenCV", "Python", "Face Recognition", "SQLite", "HTML/CSS", "JavaScript"],
      github: "https://github.com/Face-Attendances/Face-Attendance",
      demo: "#",
      icon: "👤"
    },
    {
      title: "Dental Management System",
      description: "Hệ thống quản lý nha khoa toàn diện với API RESTful. Quản lý bệnh nhân, lịch hẹn, điều trị và báo cáo. Sử dụng JWT authentication và thiết kế responsive.",
      techStack: ["Django REST", "JWT", "Python", "API Development", "MySQL", "Authentication"],
      github: "https://github.com/fourgay/dental-system",
      demo: "#",
      icon: "🦷"
    },
    {
      title: "Fruit Management System",
      description: "Ứng dụng quản lý trái cây với giao diện thân thiện. Quản lý kho hàng, đơn hàng, và khách hàng. Tích hợp hệ thống báo cáo và thống kê chi tiết.",
      techStack: ["Django", "MySQL", "Bootstrap", "Chart.js", "Python", "Web Development"],
      github: "https://github.com/Fruits-manage/QLDA",
      demo: "#",
      icon: "🍎"
    }
  ]

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

  const cardVariants = {
    hidden: { y: 50, opacity: 0 },
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
    <ProjectsSection id="projects">
      <Container>
        <SectionTitle
          initial={{ opacity: 0, y: -50 }}
          whileInView={{ opacity: 1, y: 0 }}
          transition={{ duration: 0.8 }}
          viewport={{ once: true }}
        >
          Dự Án
        </SectionTitle>

        <motion.div
          variants={containerVariants}
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true }}
        >
          <ProjectsGrid>
            {projects.map((project, index) => (
              <ProjectCard
                key={index}
                variants={cardVariants}
                whileHover={{ scale: 1.02 }}
              >
                <ProjectImage>
                  <div className="project-overlay">
                    <div className="overlay-content">
                      <a href={project.github} target="_blank" rel="noopener noreferrer">
                        <i className="fab fa-github"></i>
                      </a>
                      <a href={project.demo} target="_blank" rel="noopener noreferrer">
                        <i className="fas fa-external-link-alt"></i>
                      </a>
                    </div>
                  </div>
                </ProjectImage>

                <ProjectContent>
                  <ProjectTitle>{project.title}</ProjectTitle>
                  <ProjectDescription>{project.description}</ProjectDescription>

                  <TechStack>
                    {project.techStack.map((tech, techIndex) => (
                      <TechTag key={techIndex}>{tech}</TechTag>
                    ))}
                  </TechStack>

                  <ProjectLinks>
                    <a href={project.github} target="_blank" rel="noopener noreferrer">
                      <i className="fab fa-github"></i>
                      GitHub
                    </a>
                    <a href={project.demo} className="secondary" target="_blank" rel="noopener noreferrer">
                      <i className="fas fa-eye"></i>
                      Demo
                    </a>
                  </ProjectLinks>
                </ProjectContent>
              </ProjectCard>
            ))}
          </ProjectsGrid>
        </motion.div>
      </Container>
    </ProjectsSection>
  )
}

export default Projects