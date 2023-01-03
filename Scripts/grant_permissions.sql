create role readwrite;

grant connect on database subscriptions to readwrite;
grant usage on schema public to readwrite;
grant SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO readwrite;

create user subscription_app_user with password 'Sr1Lank4!';
grant readwrite to subscription_app_user;

