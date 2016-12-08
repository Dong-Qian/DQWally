-- Ownner:       Dong Qian (6573448)
-- Script:       RD assignment 04
-- Due Date:     Dec 3, 2016
-- Instructor:   Russell Foubert
-- Description:  Start Data Base for RevengeOfWally assignemnt


CREATE DATABASE DQWally;

USE DQWally;

grant all privileges on dqwally.* to 'root'@'%' identified by 'Conestoga1';

#Create the tables
#customer table

CREATE TABLE customer 
(
	customerID BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL, 
	firstName VARCHAR(45),
	lastName VARCHAR(45), 
	phoneNumber VARCHAR(45) NOT NULL
);

# branch table
CREATE TABLE branch 
(
	branchID BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL, 
	branchName VARCHAR(45) NOT NULL
);

# product table
CREATE TABLE product
(
	productID BIGINT PRIMARY KEY AUTO_INCREMENT, 
	productName VARCHAR(45) NOT NULL, 
	unitPrice DECIMAL(10,2), 
	inventoryQuantity BIGINT
);

# orders table
CREATE TABLE orders 
(
	ordersID BIGINT PRIMARY KEY AUTO_INCREMENT NOT NULL,
	orderDate DATETIME NOT NULL, 
	customerID BIGINT, 
	branchID BIGINT, 
	orderStatus VARCHAR(10),
	FOREIGN KEY (customerID) REFERENCES customer(customerID),
	FOREIGN KEY (branchID) REFERENCES branch(branchID)
);

# orderline table
CREATE TABLE orderline 
(
	ordersID BIGINT, 
	productID BIGINT, 
	quantity BIGINT,
	FOREIGN KEY (ordersID) REFERENCES orders(ordersID),
	FOREIGN KEY (productID) REFERENCES product(productID)
);
                        
                        
# insert 3 customer to customer table
INSERT INTO customer (firstName, lastName, phoneNumber) 
VALUES 	('Nobert', 'Mika', '416-555-1111'),
		('Russell', 'Foubert', '519-555-2222'),
		('Sean', 'Clarke', '519-555-3333');


# insert 3 branch into branch table
INSERT INTO branch (branchName) 
VALUES 	('Sports World'),
		('Cambridge Mall'),
		('St.jacobs');
                                       

# insert 6 product into product table
INSERT INTO product (productName, unitPrice, inventoryQuantity) 
VALUES 	('Disco Queen Wallpaper (roll)', 12.95, 56),
		('Countryside Wallpaper (roll)', 11.95, 24),
		('Victorian Lace Wallpaper (roll)', 14.95, 44),
		('Drywall Tape (roll)', 3.95, 120),
		('Drywall Tape (pkg 10)', 36.95, 30),
		('Drywall Repair Compound (tube)', 6.95, 90);


# insert 3 orders to orders table
INSERT INTO orders(orderDate, customerID, branchID, orderStatus) 
VALUES 	('2016-09-20', 3, 1, 'PAID'),
		('2016-10-06', 2, 2, 'PEND'),
		('2016-11-02', 1, 3, 'PAID');
                                     
									 
# insert orderline for sean's order
INSERT INTO orderline (ordersID, productID, quantity) 
VALUES 	(1, 3, 4),
		(1, 6, 1),
		(1, 4, 2 );

                                                        
# change InventoryQuantity for Sean's order
UPDATE product 
SET inventoryQuantity = inventoryQuantity - 4 
WHERE productID = 3;

UPDATE product 
SET inventoryQuantity = inventoryQuantity - 1 
WHERE productID = 6;

UPDATE product 
SET inventoryQuantity = inventoryQuantity - 2 
WHERE productID = 4;


# insert orderline for Russell's order										
INSERT INTO orderline (ordersID, productID, quantity) 
VALUES (2, 2, 10);


# insert orderline for Norbert's order
INSERT INTO orderline (ordersID, productID, quantity) 
VALUES 	(3, 1, 12),
		(3, 4, 3);


# change InventoryQuantity for Norbert's order 
UPDATE product 
SET inventoryQuantity = inventoryQuantity - 12 
WHERE productID = 1;


UPDATE product 
SET inventoryQuantity = inventoryQuantity - 3 
WHERE productID = 4;


# change the order status for Russel's pend order
UPDATE orders 
SET orderStatus = 'CNCL' 
WHERE ordersID = 2;


# zero out the orderline
UPDATE orderline 
SET quantity = 0 
WHERE ordersID = 2 AND 
	  productID = 2 AND 
	  quantity = 10;

															
# change the order status fro Norber's order
UPDATE orders 
SET orderStatus = 'RFND' 
WHERE ordersID = 3;


# adjust the inventory
UPDATE product 
SET inventoryQuantity = inventoryQuantity + 12 
WHERE productID = 1;

UPDATE product 
SET inventoryQuantity = inventoryQuantity + 3 
WHERE productID = 4;


# zero out the orderline
UPDATE orderline 
SET quantity = 0 
WHERE ordersID = 3 AND 
	  productID = 1 AND  
	  quantity = 12;

UPDATE orderline 
SET quantity = 0 
WHERE ordersID = 3 AND 
	  productID = 4 AND  
	  quantity = 3;
												