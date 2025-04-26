CREATE TABLE user_groups(
	id int identity(1, 1) primary key,
	name nvarchar(255) not null,
	status bit not null default 1,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
)

CREATE TABLE users(
	id int identity(1, 1) primary key,
	fullname nvarchar(255) null,
	username nvarchar(255) not null,
	password nvarchar(255) not null,
	user_group_id int not null,
	status bit not null default 1,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (user_group_id) references user_groups(id)
)

CREATE TABLE roles(
	id int identity (1, 1) primary key,
	name nvarchar(255) not null,
	status bit not null default 1,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null
)

CREATE TABLE user_roles(
	user_id int,
	role_id int,
	primary key (user_id, role_id),
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (user_id) references users(id),
	foreign key (role_id) references roles(id)
)

CREATE TABLE permissions(
	id int identity (1, 1) primary key,
	name nvarchar(255) not null,
	description nvarchar(255) not null,
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null
)

CREATE TABLE role_permissions(
	role_id int,
	permission_id int,
	primary key (role_id, permission_id),
	created_at datetime default getdate() not null,
	updated_at datetime default getdate() not null,
	foreign key (role_id) references roles(id),
	foreign key (permission_id) references permissions(id)
)