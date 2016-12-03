CREATE DATABASE DQWally;

USE DQWally;

grant all privileges on dqwally.* to 'superuser'@'%' identified by 'Conestoga1';

CREATE TABLE customer (CustomerID BIGINT PRIMARY KEY AUTO_INCREMENT, FirstName VARCHAR(45),
					   LastName VARCHAR(45), PhoneNumber VARCHAR(45));
			
CREATE TABLE branch (BranchID BIGINT PRIMARY KEY AUTO_INCREMENT, BranchName VARCHAR(45));

CREATE TABLE product (ProductID BIGINT PRIMARY KEY AUTO_INCREMENT, ProductName VARCHAR(45), 
					  Price DECIMAL(10,2), InventoryQuantity BIGINT);
                      
CREATE TABLE orders (OrdersID BIGINT PRIMARY KEY AUTO_INCREMENT, OrderDate DATETIME, 
					 CustomerID BIGINT, BranchID BIGINT, OrderStatus VARCHAR(10),
                     FOREIGN KEY (CustomerID) REFERENCES customer(CustomerID),
					 FOREIGN KEY (BranchID) REFERENCES branch(BranchID));

CREATE TABLE orderline (OrdersID BIGINT, ProductID BIGINT, Quantity BIGINT,
						FOREIGN KEY (OrdersID) REFERENCES orders(OrdersID),
						FOREIGN KEY (ProductID) REFERENCES product(ProductID));
                        
                        
                        
INSERT INTO customer (FirstName, LastName, PhoneNumber) VALUES ('Nobert', 'Mika', '416-555-1111'),
															   ('Russell', 'Foubert', '519-555-2222'),
															   ('Sean', 'Clarke', '519-555-3333');



INSERT INTO branch (BranchName) VALUES ('Sports World'),
									   ('Cambridge Mall'),
									   ('St.jacobs');
                                       
#SELECT * FROM branch;

INSERT INTO product (ProductName, Price, InventoryQuantity) VALUES ('Disco Queen Wallpaper (roll)', 12.95, 56),
																   ('Countryside Wallpaper (roll)', 11.95, 24),
																   ('Victorian Lace Wallpaper (roll)', 14.95, 44),
																   ('Drywall Tape (roll)', 3.95, 120),
																   ('Drywall Tape (pkg 10)', 36.95, 30),
																   ('Drywall Repair Compound (tube)', 6.95, 90);
#SELECT * FROM product;


INSERT INTO orders(OrderDate, CustomerID, BranchID, OrderStatus) VALUES ('2016-09-20', 3, 1, 'PAID'),
																		('2016-10-06', 2, 2, 'PEND'),
                                                                        ('2016-11-02', 1, 3, 'PAID');
                                                                        
#select * from orders;

#PAID Sean
INSERT INTO orderline (OrdersID, ProductID, Quantity) VALUES (1, 3, 4),
															 (1, 5, 1),
															 (1, 4, 2 );
#select * from orderline;

#change InventoryQuantity
UPDATE product SET InventoryQuantity = InventoryQuantity - 4 WHERE ProductID = 3;
UPDATE product SET InventoryQuantity = InventoryQuantity - 1 WHERE ProductID = 5;
UPDATE product SET InventoryQuantity = InventoryQuantity - 2 WHERE ProductID = 4;


#select * from product;


#pending Russell												
INSERT INTO orderline (OrdersID, ProductID, Quantity) VALUES (2, 2, 10);
#cancel
UPDATE orders SET OrderStatus = 'CNCL' WHERE OrdersID = 2;


#PAID Norbert
INSERT INTO orderline (OrdersID, ProductID, Quantity) VALUES (3, 1, 12),
															 (3, 4, 3);
															
#change InventoryQuantity
UPDATE product SET InventoryQuantity = InventoryQuantity - 12 WHERE ProductID = 1;
UPDATE product SET InventoryQuantity = InventoryQuantity - 3 WHERE ProductID = 4;

#refund
UPDATE orders SET OrderStatus = 'RFND' WHERE OrdersID = 3;
UPDATE product SET InventoryQuantity = InventoryQuantity + 12 WHERE ProductID = 1;
UPDATE product SET InventoryQuantity = InventoryQuantity + 3 WHERE ProductID = 4;




													