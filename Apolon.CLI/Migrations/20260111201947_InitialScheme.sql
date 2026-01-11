-- MIGRATION: 20260111201947_InitialScheme.sql

-- UP
CREATE TABLE IF NOT EXISTS patients (
    patient_id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    date_of_birth TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS medications (
    medication_id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    manufacturer VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS prescriptions (
    prescription_id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL,
    medication_id INTEGER NOT NULL,
    dosage VARCHAR(100) NOT NULL,
    start_date TIMESTAMP NOT NULL,
    
    CONSTRAINT fk_prescriptions_patient_id FOREIGN KEY (patient_id) REFERENCES patients(patient_id),
    CONSTRAINT fk_prescriptions_medication_id FOREIGN KEY (medication_id) REFERENCES medications(medication_id)
);

CREATE TABLE IF NOT EXISTS checkups (
    checkup_id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL,
    checkup_date TIMESTAMP NOT NULL,
    notes TEXT,
    
    CONSTRAINT fk_checkups_patient_id FOREIGN KEY (patient_id) REFERENCES patients(patient_id)
);

-- DOWN
DROP TABLE IF EXISTS checkups;
DROP TABLE IF EXISTS prescriptions;
DROP TABLE IF EXISTS medications;
DROP TABLE IF EXISTS patients;