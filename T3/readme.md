# readme

a appoint make prog for self-employed small bussiness

##todo features
1. loading 的UI提示
1.templates/模板、演示数据、数据复位（将过期数据清除、导出），自动载入等等
1. 演示数据：包括class等（重复）:excel 数据导入、存储到DB、再解析执行，个人照片信息导入并显示  
1. 模板：主要是面对不同行业用户整理的
1. 数据复位：运行时间较长后，清理复位，重新开始

1. 数据迁移：发布数据库更新？
1. 测试：unit testing；界面的实际操作测试？（很重要）

wechat
1. 微信授权（将微信用户信息授权给第三方系统）
2. 打开第三方网页（菜单连接，需要“认证”公众号才行吧，此项已确认：必须）
3. 

1. Cache的应用：base64直接发送图片这种方式，是否废除了cache？
1.Multi Tenant
1. 还应该有各种信息、设置、特定规则等

1. wechat 授权登录（仅授权获取用户信息）
1.authorization & authentication(Claims based)
1.cloud based
1. DI/CI?自动的测试和发布
1. 缺省的多界面CRUD方式要改为跟方便的
1. 作废、还是删除？
1. logs

1. wechat login
1. auto task
6. Unit test & 页面测试发布测试
7. auto deploy automation
8. .io域名wechat可以打开？还是需要先申报？

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

## 广告可以带来什么？





