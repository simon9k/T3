# readme

a appoint make prog for selfemployed small bussiness


1. Multi Tenant 
1. authorization & authentication(Claims based)
1. cloud based
1. wechat login
1. auto task
6. Unit test & 页面测试发布测试
7. auto deploy automation
8. 

Claims based authentication
1. fundement policy setting
1. runtime customize policy by AppUser's admin

参考：
.net core documents have full introduce about authorization, feel can fully
support my app's situation.

Implementing authorization in web applications and APIs - Dominick Baier & Brock Allen
动态生成policy，authenticationClient, can DI inject to Controller

# todo
1. Tenant:AppUser 应该是多对多关系，需要设立TenantUserEnrollment关联机制

## 登录及注册
网页：
扫描登录->个人页面
扫描登录->没有注册->创建机构及管理员页面->管理页面
扫描登录->没有注册->加入机构及注册->个人页面

## 学员和家长对应机制
请关注公众号（含机构），并登记学员信息（系统创建学员）+认领，
请关注公众号（含机构），已登记学员信息，认领，
认领： 姓名、小名、性别、出生年月（通知其他认领人）

## 关注公众号
= 注册+加入机构

## cookie cache，如何保持登录（wechat）
Identity login state keeping mechani

## log
structure log


### For New ,Start with Razor Pages
Web app	For new development	Get started with Razor Pages

### multiTenant
all data set with TenantId
Enrollment/CourseAssignment get TenantId field




