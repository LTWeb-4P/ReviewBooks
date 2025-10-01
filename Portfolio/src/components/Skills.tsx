import React from 'react'
import { motion } from 'framer-motion'
import styled from 'styled-components'

const SkillsSection = styled.section`
  padding: 100px 0;
  background: linear-gradient(135deg, var(--primary-color) 0%, var(--secondary-color) 100%);
  color: white;
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
  color: white;
`

const SkillsGrid = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin-top: 3rem;
`

const SkillCategory = styled(motion.div)`
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  padding: 2rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
`

const CategoryTitle = styled.h3`
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  color: white;
  text-align: center;
`

const SkillTags = styled.div`
  display: flex;
  flex-wrap: wrap;
  gap: 0.8rem;
  justify-content: center;
`

const SkillTag = styled(motion.span)`
  background: rgba(255, 255, 255, 0.2);
  color: white;
  padding: 8px 16px;
  border-radius: 25px;
  font-size: 0.9rem;
  font-weight: 500;
  border: 1px solid rgba(255, 255, 255, 0.3);
  transition: all 0.3s ease;
  
  &:hover {
    background: rgba(255, 255, 255, 0.3);
    transform: translateY(-2px);
    box-shadow: 0 5px 15px rgba(0, 0, 0, 0.2);
  }
`

const Skills: React.FC = () => {
    const skillCategories = [
        {
            title: "Frontend Development",
            skills: ["React", "TypeScript", "JavaScript", "HTML5", "CSS3", "Tailwind CSS", "Bootstrap", "jQuery"]
        },
        {
            title: "Backend Development",
            skills: ["Django", "Django REST", "Python", "Node.js", "Express.js", "JWT Authentication", "API Development"]
        },
        {
            title: "Database & Tools",
            skills: ["MySQL", "PostgreSQL", "SQLite", "Git", "GitHub", "VS Code", "Postman", "Docker"]
        },
        {
            title: "AI & Computer Vision",
            skills: ["OpenCV", "Face Recognition", "Image Processing", "Machine Learning", "TensorFlow", "scikit-learn"]
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

    const categoryVariants = {
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

    const tagVariants = {
        hidden: { scale: 0, opacity: 0 },
        visible: {
            scale: 1,
            opacity: 1,
            transition: {
                duration: 0.5,
                ease: "easeOut"
            }
        }
    }

    return (
        <SkillsSection id="skills">
            <Container>
                <SectionTitle
                    initial={{ opacity: 0, y: -50 }}
                    whileInView={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.8 }}
                    viewport={{ once: true }}
                >
                    Kỹ Năng
                </SectionTitle>

                <motion.div
                    variants={containerVariants}
                    initial="hidden"
                    whileInView="visible"
                    viewport={{ once: true }}
                >
                    <SkillsGrid>
                        {skillCategories.map((category, index) => (
                            <SkillCategory
                                key={index}
                                variants={categoryVariants}
                                whileHover={{ scale: 1.02 }}
                            >
                                <CategoryTitle>{category.title}</CategoryTitle>
                                <SkillTags>
                                    {category.skills.map((skill, skillIndex) => (
                                        <SkillTag
                                            key={skillIndex}
                                            variants={tagVariants}
                                            whileHover={{ scale: 1.1 }}
                                            whileTap={{ scale: 0.95 }}
                                        >
                                            {skill}
                                        </SkillTag>
                                    ))}
                                </SkillTags>
                            </SkillCategory>
                        ))}
                    </SkillsGrid>
                </motion.div>
            </Container>
        </SkillsSection>
    )
}

export default Skills