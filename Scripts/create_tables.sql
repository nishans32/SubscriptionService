CREATE TABLE subscriptions.public.users
(
  id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  externaluserid uuid NOT NULL ,
  name VARCHAR(100) NOT NULL,
  email VARCHAR(50) UNIQUE,
  salary MONEY,
  expenses MONEY,
  datecreated TIMESTAMP
);

CREATE INDEX ix_users_externalid
  ON subscriptions.public.users USING btree(externaluserid);


CREATE TABLE subscriptions.public.useraccount
(
  id INT GENERATED ALWAYS AS IDENTITY,
  accounttype INT,
  userid INT references users(id),
  loanamount MONEY,
  interestrate NUMERIC(5,2),
  repaymentamount MONEY,
  repaymentfrequency INT,
  datecreated TIMESTAMP
);
