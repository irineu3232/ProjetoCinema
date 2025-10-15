-- Criando database
drop database if exists dbCinema;
create database dbCinema;
use dbCinema;


create table Usuario (
id int primary key auto_increment,
Nome varchar(100) not null,
Email varchar(150) not null,
Senha varchar(100) not null,
role enum ('admin','gerente','cliente'),
Ativo varchar(1) not null
);


create table Diretores(
id_diretor int primary key auto_increment,
nome varchar(100) not null,
pais_origem varchar(100) not null
);

create table Filmes(
id_filme int primary key auto_increment,
titulo varchar(100) not null,
genero int,
id_diretor int,
foreign key (id_diretor) references Diretores(id_diretor)
);

create table Filmes_Genero(
id_Gen int primary key auto_increment,
nomeGen varchar(100)
);


create table Premiacoes(
id_premiacao int primary key auto_increment,
id_filme int,
nomePremio varchar(100) not null,
foreign key(id_filme) references Filmes(id_filme)
);



describe Filmes;
describe Filmes_Genero;
describe Diretores;


-- Criando procedures
Delimiter $$
create procedure cad_Usuario(u_nome varchar (100), u_email varchar(100), u_senha varchar(100), u_role varchar(40))
begin

	if not exists (select id from Usuario where Email = u_email)
					then
		insert into Usuario(Nome, Email, Senha, role, Ativo)
						values(u_nome, u_email, u_senha, u_role, 1);
	end if;
end $$

Delimiter $$
create procedure editar_usuario(u_nome varchar (100), u_email varchar(100), u_senha varchar(100), u_role varchar(40), id_user int)
begin
			
            Update Usuario
            set Nome = u_nome, Email = u_email, Senha = u_senha, role = u_role
            where id = id_user;

end $$


Delimiter $$
create procedure deletar_usuario(u_id int)
begin
	delete from Usuario where id = u_id;
end $$

Delimiter $$
create procedure buscar_usuario_login (u_email varchar(150))
begin

	select Id, Nome , Email, Senha, role, Ativo from Usuarios
    where Email = u_email;

end $$


Delimiter $$
create procedure listar_usuario ()
begin
	select Id, Nome, Email, Senha, role, Ativo from Usuario;
end $$



Delimiter $$
drop procedure if exists obter_usuario $$
create procedure obter_usuario(u_id int)
begin

	select Id,Nome,Email,Senha,role from Usuario
	where Id = u_id;

end $$



-- Criando Filmes
describe Filmes;

Delimiter $$
create procedure cad_Filme(f_titulo varchar (100), f_genero int, f_diretor int)
begin

	if not exists (select id_filme from Filme where titulo = f_titulo)
					then
		insert into Filmes(titulo, genero, id_diretor)
						values(f_titulo, f_genero, f_diretor);
	end if;
end $$

Delimiter $$
create procedure editar_filme(f_titulo varchar (100), f_genero int, f_diretor int, f_cod int)
begin
			
            Update Filmes
            set titulo = f_titulo, genero = f_genero, id_diretor = f_diretor
            where id_filme = f_cod;

end $$


Delimiter $$
create procedure deletar_filme(f_id int)
begin
	delete from Filmes where id_filme = f_id;
end $$



Delimiter $$
create procedure listar_Filme()
begin
	select f.id_filme, f.titulo, f.id_diretor, f.genero from Filmes f
    inner join Diretores d on f.id_diretor = d.id_diretor
    inner join Genero g on f.genero = g.id_gen;

end $$


Delimiter $$
drop procedure if exists obter_filme $$
create procedure obter_filme(f_id int)
begin
	Select id_filme, titulo, genero , id_diretor from Filmes
    where id_filme = f_id;

end $$

-- Premiacao

Delimiter $$
create procedure cad_premiacao(p_nomePremio varchar (100), p_filme int)
begin

		insert into Premiacoes(id_filme, nomePremio)
						values(p_filme, p_nomePremio);

end $$

Delimiter $$
create procedure editar_premiacao(p_nomePremio varchar (100), p_filme int, p_id int)
begin
			
            Update Premiacoes
			set nomePremio = p_nomePremio, id_filme = p_filme
            where id_premiacao = p_id;
end $$


Delimiter $$
create procedure deletar_premiacao(p_id int)
begin
	delete from Premiacoes where id_premiacao = p_id;
end $$


Delimiter $$
Drop procedure if exists buscar_premiacao $$
create procedure buscar_premiacao(In p_q varchar(200), in c_t varchar(200))
begin
	Select 
	 f.id_filme, f.titulo
    from Filmes f 
    inner join Premiacoes p on f.id_filme = p.id_filme
    inner join Filmes_Genero g on f.genero = g.id_Gen
    where 
		(p_q is null or p_q = '' or l.titulo like concat('%', p_q, '%'))
        or
        (c_t is null or c_t = '' or g.nomeGen like concat('%',c_t,'%'))
	Order by f.titulo;
end $$




-- Diretores --

Delimiter $$
create procedure cad_Diretor(d_nome varchar (100), d_pais varchar(100))
begin

	if not exists (select id_diretor from Diretores where nome = d_nome )
					then
		insert into Diretores(nome, pais_origem)
						values(d_nome, d_pais);
	end if;
end $$

Delimiter $$
create procedure editar_diretor(d_nome varchar (100), d_pais varchar(100), id_di int)
begin
			
            Update Diretores
            set nome = d_nome, pais_origem = d_pais
            where id_diretor = id_di;

end $$


Delimiter $$
create procedure deletar_diretor(d_id int)
begin
	delete from Diretores where id_diretor = d_id;
end $$


delimiter $$
create procedure buscar_diretor(d_id int)
begin
	
    select id_diretor, nome, pais_origem
    from Diretores where id_diretor = d_id;
	
end $$

delimiter $$
create procedure listar_diretor()
begin
	
		select id_diretor, nome, pais_origem
        from Diretores;
    
end $$


-- CADASTRO DE GÊNERO DE FILMES!!!!!!!


Delimiter $$
create procedure cad_genero(g_nome varchar(100))
begin

	if not exists (select id_Gen from Filmes_Genero where nome = g_nome )
					then
		insert into Filmes_Genero(nomeGen)
						values(g_nome);
	end if;
end $$

Delimiter $$
create procedure editar_genero(g_nome varchar (100), g_id int)
begin
			
            Update Filmes_Genero
            set nomeGen = g_nome
            where id_Gen = g_id;

end $$


Delimiter $$
create procedure deletar_genero(g_id int)
begin
	delete from Filmes_Genero where id_Gen = g_id;
end $$


delimiter $$
create procedure buscar_genero(g_id int)
begin
	
    select nomeGen, id_Gen
    from Filmes_Genero where id_Gen = g_id;
	
end $$

delimiter $$
create procedure listar_genero()
begin
	
		select id_Gen, nomeGen
        from Filmes_Genero;
    
end $$





describe Diretores;
describe Filmes;
describe Filmes_Genero;
describe Premiacoes;


