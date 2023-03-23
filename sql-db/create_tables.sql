-- Creation of patients table
CREATE TABLE patients    
(    
  id character varying(50) NOT NULL,    
  name character varying(200) NOT NULL,    
  address character varying(500),    
  city character varying(100),    
  age numeric NOT NULL,    
  gender character varying(10),    
  CONSTRAINT patient_pkey PRIMARY KEY (id)    
); 

-- Creation of product table
CREATE TABLE IF NOT EXISTS product (
  product_id INT NOT NULL,
  name varchar(250) NOT NULL,
  PRIMARY KEY (product_id)
);

-- INSERT INTO patients (id, name, address,city,age,gender) VALUES 
-- (99302414-6000-42c1-b9f5-2597091b26b4,'Mukund Jha','Mumbai','Thane',30,'Male'), 
-- (823d581d-9387-4f66-ad73-b0091c4442d3,'Suresh','Mumbai','Thane',30,'Male');