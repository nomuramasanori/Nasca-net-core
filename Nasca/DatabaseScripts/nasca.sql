DROP TABLE m_element;
DROP TABLE t_depndncy;
DROP TABLE m_elmttype;
/
CREATE TABLE m_element (
  elmtid varchar(200) NOT NULL,
  elmtnm varchar(40) DEFAULT NULL,
  elmttp varchar(20) DEFAULT NULL,
  remark varchar(200) DEFAULT NULL,
  PRIMARY KEY (elmtID)
)
/
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('1','1','excel','input remark');
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('1.3','3','csv','input remark');
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('1.3.4','4','csv','input remark');
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('2','2','excel','input remark');
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('5','5','csv','input remark');
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('5.6','6','excel','input remark');
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('5.y','y','excel','input remark');
insert into "public".m_element(elmtid,elmtnm,elmttp,remark) values ('r','r','csv','input remark');

/
CREATE TABLE m_elmttype (
  elmttp varchar(20) NOT NULL,
  svgfle varchar(60) NOT NULL,
  PRIMARY KEY (elmttp)
)
/
insert into "public".m_elmttype(elmttp,svgfle) values ('csv','file-csv');
insert into "public".m_elmttype(elmttp,svgfle) values ('excel','excel');
/
CREATE TABLE t_depndncy (
  elmtid varchar(200) NOT NULL,
  dpdeid varchar(200) NOT NULL,
  dpdtpc boolean NOT NULL,
  dpdtpr boolean NOT NULL,
  dpdtpu boolean NOT NULL,
  dpdtpd boolean NOT NULL,
  remark varchar(200) DEFAULT NULL,
  PRIMARY KEY (elmtid,dpdeid)
)
/
insert into "public".t_depndncy(elmtid,dpdeid,dpdtpc,dpdtpr,dpdtpu,dpdtpd,remark) values ('5.6','1.3.4',True,False,False,False,'input remark');
insert into "public".t_depndncy(elmtid,dpdeid,dpdtpc,dpdtpr,dpdtpu,dpdtpd,remark) values ('5.6','2',True,False,False,False,'input remark');
insert into "public".t_depndncy(elmtid,dpdeid,dpdtpc,dpdtpr,dpdtpu,dpdtpd,remark) values ('5.y','1.3.4',False,True,False,False,'input remark');
insert into "public".t_depndncy(elmtid,dpdeid,dpdtpc,dpdtpr,dpdtpu,dpdtpd,remark) values ('5.y','5.6',False,False,False,True,'input remark');
insert into "public".t_depndncy(elmtid,dpdeid,dpdtpc,dpdtpr,dpdtpu,dpdtpd,remark) values ('r','2',True,False,False,False,'input remark');
/
