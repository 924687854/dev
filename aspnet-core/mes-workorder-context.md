# MES 工单模块上下文文档

## 1. 文档目的

本文件用于持续记录 MES 工单模块的需求确认过程、当前有效业务规则、设计决策、执行进度和待办事项。
后续无论由哪个 AI 接手，只需阅读本文件即可快速理解当前背景和进展。

本文档记录到“下一个 AI 看了就知道要做什么、按什么业务规则做”的程度即可，不追求写到可直接照抄的代码级细节。具体可执行细节（字段类型、命名、报错码、索引等）在真正写代码时再逐步确认。

## 2. 用户协作规则

以下规则由用户明确提出，后续设计和实现必须遵守：

1. 每次只做用户明确要求的事，不超前实现。
2. 写代码前先确认需求，并列出假设。
3. 只使用确实存在的库和 API；不确定时明确说明“不确定，需要核实”。
4. 每段代码必须可独立运行，包含异常处理，并带有必要注释。
5. 写完后主动说明依赖、测试方法、不确定点。
6. 报错时先诊断根因，再提出修复方案。
7. 如果需求不清晰，先反问，不猜测。
8. 除本文件外，其他代码改动都必须先经过用户同意。
9. 当用户提出的新要求与本文档已确认的规则或决策冲突时，必须主动提醒用户，明确指出会与哪一条已确认内容冲突，待用户确认后再修改，不得默默覆盖或忽略。
10. 采用"一个问题一个问题确认"的协作方式推进。
11. 严格按照 ABP 框架官方规范和项目已有约定执行，不确定的必须先问用户，不得自行假设或跳过。包括但不限于：表名前缀（当前项目为 App）、DbContext 配置方式、实体基类选择、仓储模式、权限定义方式等。

## 3. 当前项目背景

- 项目路径：`/Users/wanghengning/Downloads/code/dev/aspnet-core`
- 技术栈：`.NET + ABP`
- 当前状态：项目是标准 ABP 分层后端骨架，暂未发现现成 MES / 工单业务模块
- 当前范围：只做后端，暂不考虑前端
- 当前实施原则：未获用户明确授权前，不修改任何业务代码、数据库、接口或测试文件

## 4. 当前最终有效设计摘要

1. 首版只做 MES 工单模块中的生产工单能力。
2. 首版只有“生产工单”这一张业务单据，采用单据头/体结构。
3. 不设置独立的“工序工单”单据或实体。
4. 工序以“工序明细行”形式挂在生产工单下。
5. “分解”指在同一张生产工单内，根据工艺路线生成工序明细行，而不是派生新单据。
6. 生产工单建单成功后即为“已下达”状态，ERP 接口传入的工单同样默认已下达。
7. 创建生产工单时系统自动匹配工艺路线；匹配到则自动关联，匹配不到仍允许建单。
8. 未匹配到工艺路线的生产工单，在分解生成工序明细行前必须人工补齐并确认工艺路线。
9. 一张生产工单只允许绑定一个工艺路线版本。
10. 一旦分解生成工序明细行，生产工单上的工艺路线版本锁定，不允许直接修改。
11. 如需修改已分解生产工单的工艺路线，必须先删除已生成的工序明细行，再修改路线并重新分解；仅未开工的工序明细行允许删除。
12. 首版生产工单状态为：已下达、已分解、进行中、已完成。
13. 首版不做作废、关闭、暂停等异常状态。

## 5. 已确认需求与业务规则

### 5.1 总体目标

在当前后端项目基础上，从零开始设计并逐步实现 MES 的工单模块，参考金蝶云星空的工单设计思路，但不要求 1:1 复制。

### 5.2 当前明确范围

- 只聚焦生产工单功能。
- 基于现有仓库直接新增模块。
- 暂不做前端。
- 首版目标优先做到：
  - 数据模型
  - 状态流转
  - 创建接口
- 首版创建接口范围：
  - 仅开放生产工单创建。
  - 工序以明细行形式挂在生产工单下。
  - 工序明细行由生产工单“分解”动作生成。
  - ERP 传入 MES 的生产工单默认处于已下达状态。
  - 手动下达一般只出现在测试场景。

### 5.3 生产工单结构

生产工单采用单头单产品模型。

#### 表头核心字段

首版生产工单表头核心业务字段为：

1. 工单号，MES 自生成。
2. 单据状态。
3. 产品编码。
4. 计划数量。
5. 单位。
6. 生产组织。
7. 计划开工日期。
8. 计划完工日期。
9. 工艺路线 id。
10. 工艺路线号。
11. 工艺路线名称。
12. ERP 源单据 id。
13. ERP 源单据号。

补充规则：

- 工单主键及 MES 内部关联 id（如工艺路线 id）遵循 ABP 约定走 GUID。
- ERP 源单据 id 是外部系统标识，类型由 ERP 决定，不一定是 GUID，MES 原样存储用于追溯。
- ERP 源单据号用于界面或单据展示。
- 审计字段（创建人/时间、修改人/时间等）由 ABP 审计基类自带，不在业务表中自建。
- 生产工单表不含“来源”字段。
- 接口系统标识、RequestId 等接口调用追溯信息不进生产工单表，归独立接口平台层负责。

#### 表体：工序明细行

工序明细行首版字段为：

1. 工序识别码：全场唯一，作为工序执行实例的唯一标识。
2. 工序号 / 序号。
3. 工序名称。
4. 行状态：未开工、已开工、完工。
5. 计划数量。
6. 计划开工日期。
7. 计划完工日期。

补充规则：

- 工序明细行不冗余工序编码；工序编码属于主数据。
- 工序识别码是执行实例标识，区别于主数据中的工序编码。
- 工序明细行数据为“分解时从工艺路线复制的快照”。
- 工序明细行只为当前生产工单负责，生成后与工序/工艺路线主数据脱钩，主数据后续变化不影响已生成的明细行。

### 5.4 工艺路线规则

#### 自动匹配规则

创建生产工单时，系统先自动匹配工艺路线：

- 匹配维度：产品编码 + 生产组织 + 生效版本/日期。
- 匹配基准日期：系统日期，即建单当天。
- 匹配到工艺路线时，自动关联工艺路线 id、工艺路线号、工艺路线名称。
- 未匹配到工艺路线时，仍允许创建生产工单。
- 未匹配到工艺路线的生产工单，在分解生成工序明细行前必须人工补齐并确认工艺路线。
- 一张生产工单只允许绑定一个工艺路线版本。

#### 工艺路线版本链规则

- 工艺路线主数据每条有独立 id。
- 业务唯一性：产品编码 + 生产组织 + 版本号。
- 同一产品编码 + 生产组织下，任意时刻有且只有一个生效版本。
- 工艺路线存在生效日期和失效日期，形成有效区间。
- 有效期区间首尾相接不算冲突，也不留空档。
- 新版本可设置未来生效时间。
- 采用“保存即结算”机制：保存新版本时，系统自动把上一生效版本的失效日期改为新版本生效日的前一天，形成无缝版本链，不依赖定时任务。
- 到达新版本生效日期后，新创建的生产工单按匹配规则自然命中新版本；已创建的生产工单不自动切换。
- 版本链只能向未来追加，新版本生效日必须晚于当前最新版本的生效日。
- 不允许向过去插入版本，历史不可篡改。
- 失效日期不允许手动调整，只能由“保存新版本时自动结算”产生。
- 预约版本（未到生效日）的撤销与改期列入后续迭代，首版不做。

示例：

```text
A 版：2026.07.22–9999.12.31
新增 B 版预约 2026.07.31 生效
保存 B 版时，A 版自动变为 2026.07.22–2026.07.30
B 版为 2026.07.31–9999.12.31
```

#### 有效期冲突处理

- 同一产品编码 + 生产组织下，不应出现有效期重叠的多个版本同时命中。
- 如果出现多条工艺路线同时满足匹配条件，属于主数据错误，不是正常业务场景。
- 有效期冲突应在工艺路线主数据创建/维护时直接报错并阻止保存，而不是等到创建生产工单时再处理。
- 因此前提下，生产工单创建阶段默认不承担“多版本同时命中”的兜底选择逻辑。

#### 生产工单上的工艺路线修改规则

- 在未分解生成工序明细行之前，即使生产工单已处于已下达状态，也允许修改系统默认匹配的工艺路线。
- 修改生产工单上的工艺路线必须记录变更日志。
- 工艺路线变更日志首版至少记录：修改人、修改时间、修改前后路线版本、修改来源（手工/接口）。
- 修改原因字段不纳入首版，作为后续迭代内容。
- 一旦分解生成工序明细行，工艺路线版本锁定，不允许直接修改。
- 如需修改工艺路线，必须先删除已生成的工序明细行，再修改工艺路线后重新分解生成。
- 仅未开工的工序明细行允许删除；任一工序明细行已开工，首版不允许删除并重新分解。

### 5.5 状态流转规则

生产工单首版状态取值为：

1. 已下达。
2. 已分解。
3. 进行中。
4. 已完成。

正向流转：

```text
已下达 → 已分解 → 进行中 → 已完成
```

具体规则：

- 建单成功后，生产工单状态统一为已下达。
- 已下达 → 已分解：分解生成工序明细行成功。
- 已分解 → 进行中：首道工序明细行开工。
- 进行中 → 已完成：全部工序明细行完工。
- 生产工单状态允许回退。
- 回退是否受约束取决于该动作是否触及工序明细行的开工、删除等关联规则。
- 触及关联规则的回退需满足对应前置条件。例如：已分解退回已下达，需要全部工序明细行未开工，并删除全部工序明细行。
- 不触及关联规则的回退，首版仅记录原则，不逐条列死每一条回退路径。
- 首版不含作废、关闭、暂停等异常状态，列入后续迭代。

### 5.6 创建生产工单规则

#### 创建入口

手工创建与 ERP 接口创建共用同一个建单领域服务，各自只包一层薄入口。

核心建单逻辑只维护一份，包括：

1. 校验。
2. 自动匹配工艺路线。
3. 落库。
4. 单据状态置为已下达。

#### 统一建单入参

手工与 ERP 入口共用字段：

- 产品编码，必填。
- 生产组织，必填。
- 计划数量，必填。
- 单位，必填。
- 计划开工日期，必填。
- 计划完工日期，可选。

ERP 入口额外必填：

- ERP 源单据 id。
- ERP 源单据号。

#### 系统自动处理项

- 工单号由 MES 自生成，不接收外部带入。
- ERP 单号存于 ERP 源单据号。
- 单据状态置为已下达。
- 工艺路线按系统日期自动匹配。
- 匹配到工艺路线则自动关联。
- 匹配不到工艺路线仍允许建单。
- 审计字段由 ABP 自带。

#### 创建校验范围（首版）

- 必填校验：产品编码、生产组织、计划数量、单位、计划开工日期。
- ERP 入口额外必填校验：ERP 源单据 id、ERP 源单据号。
- 数值校验：计划数量必须大于 0。
- 日期校验：计划完工日期若填写，不得早于计划开工日期。
- 存在性校验：产品编码、生产组织必须在主数据中存在，否则拒绝建单。
- 存在性校验对手工与 ERP 接口来单一视同仁，采用严格口径。
- 防重校验：ERP 源单据 id 非空唯一，由数据库约束保证。
- 工艺路线：建单阶段自动匹配；匹配不到不拦截，仍允许建单。

#### ERP 源单防重

- 在数据库层面保证 MES 工单与 ERP 源单一一对应。
- ERP 源单据 id 非空唯一。
- 空值不参与唯一约束，手工工单 ERP 源单据 id 为空，不受限。
- ERP 源单据 id 是内部关联字段。
- 界面/单据仅体现 ERP 源单据号。
- ERP 源单据 id 非空唯一约束的数据库实现方式，需根据当前项目实际数据库类型确认。

### 5.7 接口审计与来源追溯

- 用户要求接口来源日志按标准审计追踪要求设计。
- 不存在唯一统一的 MES 审计字段标准，首版按通用企业审计追踪基线处理。
- 首版接口来源日志基线已确认记录到接口系统标识与 RequestId。
- 接口系统标识、RequestId 等接口调用追溯信息不进入生产工单业务表。
- 接口平台层是否在首版同步落库实现，尚待编码前确认。

### 5.8 冷启动规则

- 系统冷启动或尚无工艺路线主数据时，接口传入的生产工单仍按既有规则处理：允许建单，不因缺工艺路线而拒收。
- 此类工单在分解生成工序明细行前必须人工补齐并确认工艺路线。
- 冷启动的宽松仅适用于“工艺路线缺失”。
- 冷启动不适用于“产品编码/生产组织主数据缺失”。
- 产品编码或生产组织主数据缺失时，按存在性校验一律拒绝建单。
- 冷启动不引入额外标记或补齐机制，沿用现有规则。

## 6. 当前业务理解与假设

1. “工单”优先指生产制造场景中的生产工单。
2. 生产工单是首版唯一业务单据。
3. 工序不再作为独立单据，而是以工序明细行形式挂在生产工单下。
4. 分解是在同一张生产工单内生成工序明细行，而非派生新单据。
5. ERP 传入 MES 的生产工单默认应视为已下达。
6. 手动下达通常只用于测试场景。
7. 当前尚未确认是否涉及审批、委外、返工、拆单、合单、齐套、批次、序列号等扩展能力。
8. 当前尚未确认产品、生产组织、工艺路线等主数据实体在现有项目中的实际存在情况。
9. 当前尚未确认单位是否仅做必填校验，还是需要校验计量单位主数据或产品单位关系。
10. 当前尚未确认接口平台层在首版是否需要同步实现独立接口调用日志表。

## 7. 已完成进度

### 已完成事项

1. 已检查现有仓库结构，确认当前项目为 ABP 标准分层后端骨架。
2. 已确认本次工作将在当前仓库中从零开始新增 MES 工单模块。
3. 已确认需要维护本上下文文档，并持续更新。
4. 已确认除本文件外，其他代码改动都必须先经过用户同意。
5. 已确认首版边界优先覆盖数据模型、状态流转和创建接口。
6. 已确认首版只开放生产工单创建。
7. 已确认首版取消独立工序单据，统一采用生产工单单据头/体结构，工序以明细行形式存在。
8. 已确认工序明细行不是自动生成，而是由用户后续手动触发“分解”生成。
9. 已确认工序明细行分解生成必须依赖工艺路线，无工艺路线时首版不允许分解。
10. 已确认工艺路线自动匹配规则为产品编码 + 生产组织 + 生效版本/日期。
11. 已确认分解生成工序明细行后工艺路线版本锁定；若需修改，必须先删除工序明细行，再修改路线并重新分解生成。
12. 已确认仅未开工的工序明细行允许删除。
13. 已确认一张生产工单只允许绑定一个工艺路线版本。
14. 已确认工艺路线新版本按生效时间影响后续新工单默认匹配，已创建工单不自动切换。
15. 已确认 ERP 传入 MES 的生产工单默认处于已下达状态，手动下达一般只用于测试场景。
16. 已确认生产工单在已下达状态且未分解生成工序明细行时，允许修改默认匹配的工艺路线，但必须记录变更日志。
17. 已确认接口来源日志首版按标准审计追踪基线记录到接口系统标识与 RequestId。
18. 已确认工艺路线有效期不应重叠；若出现多版本同时命中，属于主数据错误。
19. 已确认工艺路线有效期冲突应在工艺路线主数据创建/维护时直接报错并阻止保存。
20. 已确认工艺路线采用“保存即结算”的版本链机制：保存新版本时自动结算上一版本失效日，主数据维护环节天然保证有效期不重叠。
21. 已确认工艺路线主数据失效日期不允许手动调整，仅由保存新版本时自动结算产生。
22. 已确认生产工单首版字段范围，并明确审计字段用 ABP 基类、id 走 GUID、接口追溯不进工单表。
23. 已确认工序明细行首版字段范围，并明确明细为工艺路线快照、状态由明细行联动、工序识别码全场唯一。
24. 已确认生产工单首版状态机：状态取值、正向流转、回退原则，异常态列入后续迭代。
25. 已确认生产工单创建接口方案：共用建单服务 + 双薄入口、统一入参、系统处理项、ERP 源单防重、校验范围。
26. 已根据 review 结果清理上下文文档中的旧术语和潜在冲突表述。

### 当前未开始事项

1. 第 6 步：测试尚未开始（单元测试、应用服务测试、集成测试）。
2. 分解逻辑（由用户手动触发，从工艺路线生成工序明细行快照）尚未实现。
3. 状态联动逻辑（工序开工→工单进行中、全部完工→工单已完成、回退规则）尚未实现。
4. 工艺路线修改日志记录逻辑尚未实现。
5. 工艺路线版本链"保存即结算"逻辑尚未实现。
6. HttpApi 层尚未新增显式 Controller（ABP 自动 API Controller 已可用）。
7. 接口平台日志是否首版落库尚未确认。

## 8. 当前约束

1. 本阶段只允许直接修改本文件。
2. 任何业务代码、数据库、接口、测试文件的修改，都必须先得到用户明确同意。
3. 设计范围与实施顺序分开管理，避免一次性进入过大开发面。
4. 每推进一步，先确认需求，再执行。
5. 每次确认后，优先更新本文件。

## 9. 编码前待确认问题列表

当前业务规则已基本确认完毕。已确认的问题标记为 ~~删除线~~。

1. ~~ABP 分层下的模块划分：是否新建独立 MES 模块/命名空间，还是放入现有业务模块。~~ 已确认：不新建独立模块，在现有项目内用目录/命名空间组织，去掉 Mes 前缀。
2. ~~实体划分：生产工单、工序明细行、工艺路线变更日志是否作为首批实体落地。~~ 已确认：首批落地 ProductionOrder、ProductionOrderOperation、ProductionOrderRouteChangeLog + 4 个主数据实体。
3. ~~主数据依赖：当前项目若不存在产品、生产组织、工艺路线主数据实体，首版是否需要补充最小主数据模型。~~ 已确认：先做最小主数据模型。
4. ~~单位校验：单位首版是否仅做必填校验。~~ 已确认：只做必填。
5. 接口平台日志：接口平台层是否在首版同步落库实现，还是先在 ERP 入口保留接口系统标识、RequestId 的接收和传递能力。
6. ~~ERP 源单唯一约束：根据当前项目实际数据库类型确认非空唯一约束的实现方式。~~ 已确认：MySQL UNIQUE 索引天然允许多个 NULL。
7. ~~枚举与状态命名：确认代码中的英文枚举名、数据库存储方式和展示映射。~~ 已确认：ProductionOrderStatus（int）、ProductionOrderOperationStatus（int）、RouteChangeSource（int）。
8. ~~工单号生成规则：确认 MES 工单号编码规则、前缀、流水方式和并发策略。~~ 已确认：PO+yyyyMMdd+4位流水，基于当日最大号递增。
9. ~~开发顺序：建议先领域模型，再状态机/领域服务，再应用服务和接口，最后补迁移和测试。~~ 已按此顺序执行至第 5 步完成。

## 10. ABP 编码实施指引（后续 AI 必读）

本节目标是让后续 AI 拿到本文档后，能够按照 ABP 官方推荐的分层、DDD 与应用服务习惯，知道应如何开始编码；但每一步必须先向用户确认，不能直接越权实现。

### 10.1 总体实施原则

1. 遵循 ABP 官方推荐的分层结构与 DDD 习惯。
2. 优先使用当前项目已有的模块、命名空间、基类、仓储、异常、权限、审计、租户等基础设施。
3. 不擅自引入未确认的第三方库。
4. 不擅自修改项目架构、数据库提供方、认证方式、权限体系、审计体系。
5. 写代码前必须先扫描实际仓库结构，确认项目中真实存在的层、模块名、DbContext、基类、依赖包和代码风格。
6. 如果实际项目结构与本文档假设不一致，必须先向用户说明差异并请求确认，不能强行套用。
7. 每一步只提交当前被用户确认的一小步改动，不一次性实现完整模块。
8. 每一步完成后，应说明：改了哪些文件、为什么这样改、如何测试、有哪些不确定点、下一步建议。

### 10.2 推荐 ABP 分层落点

后续 AI 应先根据真实项目结构确认具体项目名。若项目是标准 ABP 分层，通常按以下思路落地：

#### Domain 层

用于承载核心领域模型和业务规则，建议放置：

- 生产工单聚合根：`ProductionWorkOrder`。
- 工序明细行实体：`ProductionWorkOrderOperation` 或类似名称。
- 工艺路线变更日志实体：`ProductionWorkOrderRouteChangeLog` 或类似名称。
- 生产工单状态枚举。
- 工序明细行状态枚举。
- 生产工单领域服务：用于封装创建、匹配路线、分解、状态联动、路线修改等核心规则。
- 必要的领域异常或业务错误码。

注意：具体类名、命名空间、基类、接口是否需要新增，必须先向用户确认。

#### Domain.Shared 层

用于放置可被多个层引用的共享常量、枚举、错误码等。建议放置：

- 状态枚举，如项目既有习惯是放 Domain.Shared。
- 错误码常量，如项目已有 `ErrorCodes` 模式。
- MES 工单模块常量，如最大长度、模块名等。

注意：ABP 项目对枚举和错误码的放置习惯可能不同，必须先扫描现有代码后再确认。

#### Application.Contracts 层

用于定义应用服务契约和 DTO，建议放置：

- 手工创建生产工单输入 DTO。
- ERP 创建生产工单输入 DTO。
- 生产工单输出 DTO。
- 生产工单应用服务接口。
- 必要的查询 DTO，若首版需要查询。

注意：首版是否实现查询接口尚未确认，不得默认扩展。

#### Application 层

用于实现应用服务，负责编排用例，不应承载复杂领域规则。建议放置：

- 手工创建生产工单应用服务方法。
- ERP 创建生产工单应用服务方法或单独薄入口。
- 调用领域服务完成校验、路线匹配、创建和状态初始化。
- DTO 与实体映射，优先遵循项目已有 AutoMapper 或手动映射习惯。

注意：应用服务只做用例编排；核心业务规则应放在领域实体/领域服务中。

#### EntityFrameworkCore 层

用于 EF Core 映射、DbContext 扩展和仓储实现。建议放置：

- 生产工单表映射。
- 工序明细行表映射。
- 工艺路线变更日志表映射。
- ERP 源单据 id 非空唯一索引。
- 字段长度、必填、关系、索引配置。
- 如需要自定义仓储，则按项目习惯实现。

注意：索引实现方式需根据实际数据库确认。比如 SQL Server 可考虑 filtered unique index，但不能在未确认数据库类型前写死。

#### HttpApi 层

用于暴露 HTTP API Controller，前提是项目没有使用自动 API Controller 或用户要求显式 Controller。

- 如果项目使用 ABP 自动 API Controller，可不新增显式 Controller。
- 如果项目已有显式 Controller 风格，应按现有风格新增。
- 是否新增 ERP 专用接口路径，需要向用户确认。

#### HttpApi.Client 层

如项目包含该层，通常用于客户端代理。首版是否需要改动，需根据项目实际和用户要求确认。

#### DbMigrator / Migrations

如项目使用 EF Core migrations，需要在实体和映射确认后再生成迁移。生成迁移前必须向用户确认：

- 数据库提供方。
- 迁移命名。
- 是否允许本轮生成迁移文件。
- 是否只生成迁移，不执行数据库更新。

### 10.3 推荐首批代码对象（需确认后才能创建）

后续 AI 可以将以下对象作为首批候选，但不得直接创建，必须先向用户确认名称和范围。

#### 候选实体

1. `ProductionWorkOrder`
   - 生产工单聚合根。
   - 建议继承 ABP 审计聚合根基类，具体基类需按项目习惯确认。
   - 承载表头字段、状态、工艺路线关联信息、ERP 源单信息。

2. `ProductionWorkOrderOperation`
   - 生产工单下的工序明细行。
   - 建议作为聚合内子实体或关联实体，具体建模方式需确认。
   - 承载工序识别码、序号、名称、状态、计划数量和计划日期。

3. `ProductionWorkOrderRouteChangeLog`
   - 生产工单工艺路线变更日志。
   - 记录修改人、修改时间、修改前后路线版本、修改来源。
   - 是否单独建表首版落地，需用户确认。

#### 候选枚举

1. `ProductionWorkOrderStatus`
   - `Released`：已下达。
   - `Decomposed`：已分解。
   - `InProgress`：进行中。
   - `Completed`：已完成。

2. `ProductionWorkOrderOperationStatus`
   - `NotStarted`：未开工。
   - `Started`：已开工。
   - `Completed`：完工。

3. `RouteChangeSource`
   - `Manual`：手工。
   - `Interface`：接口。

枚举英文命名、数值、数据库存储方式（int/string）必须在编码前确认。

#### 候选领域服务

1. `ProductionWorkOrderManager` 或项目习惯命名的领域服务。
2. 负责：
   - 创建生产工单。
   - 校验产品编码、生产组织。
   - 匹配工艺路线。
   - 修改工艺路线并记录日志。
   - 分解生成工序明细行。
   - 根据工序明细行状态联动生产工单状态。

注意：首版已确认优先做到创建接口，分解、路线修改、状态联动是否在第一批代码中实现，需要分步确认。

#### 候选应用服务

1. `ProductionWorkOrderAppService`。
2. 可包含：
   - 手工创建方法。
   - ERP 创建方法。
3. 两个入口应共用同一个领域建单逻辑。

### 10.4 建议分步实施顺序（每一步都要确认）

后续 AI 应按小步推进，每一步开始前都向用户确认，建议顺序如下：

#### 第 0 步：扫描项目结构

目标：确认真实 ABP 项目结构。

需要查看：

- solution / csproj 列表。
- Domain、Domain.Shared、Application.Contracts、Application、EntityFrameworkCore、HttpApi 等层是否存在。
- 当前项目命名空间。
- 现有实体基类、审计基类、仓储模式。
- DbContext 名称和配置方式。
- 是否已有产品、组织、工艺路线等主数据。
- 是否已有错误码、权限、AutoMapper、Controller 风格。

完成后必须向用户汇报扫描结果，并提出第 1 步方案，不得直接写代码。

#### 第 1 步：确认模块与命名空间方案

目标：确认 MES 工单代码放在哪里。

需要向用户确认：

- 是否新建 `Mes` / `Manufacturing` / `WorkOrders` 等命名空间。
- 是否新建独立模块，还是放入现有模块。
- 实体类和 DTO 的命名。
- 是否接受首批候选对象。

用户确认后才进入第 2 步。

#### 第 2 步：确认领域模型最小闭环

目标：先落生产工单聚合根和必要枚举。

需要向用户确认：

- 实体字段。
- 字段类型。
- 字段长度。
- 必填/可空。
- 枚举命名和值。
- 聚合关系：工序明细行是 owned collection、普通子实体，还是独立表关联。
- 是否本步包含工艺路线变更日志实体。

用户确认后才可新增/修改 Domain 与 Domain.Shared 代码。

#### 第 3 步：确认 EF Core 映射与迁移

目标：配置表结构和索引。

需要向用户确认：

- 表名。
- 字段名。
- 索引。
- ERP 源单据 id 非空唯一索引实现方式。
- 是否生成 migration。
- 是否执行数据库更新。

用户确认后才可修改 EntityFrameworkCore 层和迁移文件。

#### 第 4 步：确认建单领域服务

目标：实现手工与 ERP 共用的建单领域逻辑。

需要向用户确认：

- 产品编码和生产组织存在性校验依赖哪个主数据表/服务。
- 工艺路线匹配依赖哪个主数据表/服务。
- 如果主数据不存在，是否先做最小主数据模型或临时抽象接口。
- 工单号生成规则。
- 异常与错误码规则。

用户确认后才可实现领域服务。

#### 第 5 步：确认应用服务与 DTO

目标：实现创建接口用例编排。

需要向用户确认：

- DTO 字段名。
- API 方法名。
- 返回对象。
- 手工入口和 ERP 入口是否放同一个 AppService。
- 是否需要权限控制。
- 是否需要显式 Controller。

用户确认后才可修改 Application.Contracts、Application、HttpApi 层。

#### 第 6 步：确认测试范围

目标：补充最小测试。

需要向用户确认：

- 是否编写单元测试。
- 是否编写应用服务测试。
- 是否编写集成测试。
- 使用项目已有测试框架，不引入新测试库。

用户确认后才可新增测试文件。

### 10.5 每一步向用户确认时的固定格式

后续 AI 在每一步开始前，应使用类似格式向用户确认：

```text
我准备执行第 X 步：xxx。

基于文档和当前项目扫描结果，我的假设是：
1. ...
2. ...

本步计划修改/新增的文件类型：
1. ...
2. ...

本步不会做：
1. ...
2. ...

需要你确认：
1. ...
2. ...

你确认后，我再开始修改代码。
```

如果用户只确认其中一部分，则只执行被确认的部分。

### 10.6 首版不得擅自扩展的内容

除非用户后续明确要求，首版不得擅自加入以下能力：

1. 前端页面。
2. 审批流。
3. 作废、关闭、暂停等异常状态。
4. 委外、返工、拆单、合单、齐套、批次、序列号。
5. 独立工序工单单据。
6. 自动分解工序明细行。
7. 创建生产工单时强制要求必须有工艺路线。
8. 将接口系统标识、RequestId 写入生产工单业务表。
9. 擅自引入新的第三方库。
10. 擅自改变 ABP 项目的认证、权限、审计、租户机制。

## 11. 实施进度与交接记录

本节用于保证 AI coding 暂停后，第二天更换 AI 或继续执行时，可以根据本文档和用户提供的最新代码，快速接上上次进度、要求和目标。

本节不记录版本控制信息。用户会在每次继续工作时提供最新代码，AI 必须以“本文档 + 最新代码扫描结果”共同作为接续依据。

### 11.0 强制交接规则

1. 每次 AI 开始工作前，必须先阅读本文档。
2. 每次 AI 开始工作前，必须扫描用户提供的最新代码，确认代码现状。
3. 如果本文档记录与最新代码不一致，必须暂停并向用户说明差异，不得直接继续编码。
4. 每完成一个小步骤后，必须更新本节实施进度。
5. 每次暂停前，必须记录：
   - 当前做到哪一步；
   - 已确认哪些需求；
   - 已修改哪些文件；
   - 哪些文件尚未修改；
   - 当前是否有报错；
   - 是否执行过测试；
   - 下一步应该做什么；
   - 下一位 AI 接手后第一件事是什么。
6. 如果代码已修改但未测试，必须明确标注“未测试”。
7. 如果有报错未解决，必须记录错误摘要、疑似原因和下一步排查方向。
8. 如果用户新增或修改了需求，必须同步更新到“已确认需求与业务规则”或“当前待用户确认问题”中。
9. 后续 AI 不得只依赖本节记录直接编码，必须结合最新代码重新核对。
10. 若最新代码中存在本文档未记录的 MES 工单相关改动，AI 必须先向用户说明并询问是否以代码为准更新本文档。

### 11.1 当前实施状态

当前状态：第 5 步（应用服务与 DTO）已完成，待进入第 6 步（测试）。

已完成：

- 生产工单首版业务规则确认。
- ABP 编码实施指引确认。
- 每一步需向用户确认的协作规则确认。
- AI 接续交接机制确认。
- 第 0 步：项目结构扫描已完成。
- 第 1 步：模块与命名空间方案已确认（不新建独立 ABP 模块，去掉 Mes 前缀，在现有项目内用目录/命名空间组织）。
- 第 2 步：领域模型最小闭环已落地（3 个枚举 + 3 个实体，编译通过）。
- 第 3 步：EF Core 映射与迁移已完成（3 张表映射、索引配置、迁移生成并应用到数据库）。
- 第 4 步：建单领域服务 + 最小主数据已完成（4 个主数据实体、4 张主数据表、工单号生成器、建单领域服务，编译通过，数据库已更新）。
- 第 5 步：应用服务与 DTO 已完成（权限定义、DTO、IAppService 接口、AppService 实现、AutoMapper 配置，编译通过）。

未完成：

- 第 6 步：测试。
- 尚未执行单元测试或集成测试。

### 11.2 当前所在步骤

当前位于：第 5 步已完成，待进入第 6 步（测试）。

下一步建议：执行第 6 步：测试。

第 6 步需要向用户确认：测试范围（单元测试、应用服务测试、集成测试分别做到什么程度）。

### 11.3 已完成代码改动

当前已完成第 0～5 步代码改动。

已修改文件：

- `mes-workorder-context.md`
- `src/MyProject.EntityFrameworkCore/EntityFrameworkCore/MyProjectDbContext.cs` — 新增 DbSet 和实体映射配置（含主数据）
- `src/MyProject.Application.Contracts/MyProject.Application.Contracts.csproj` — 新增 Volo.Abp.Ddd.Application.Contracts 包引用
- `src/MyProject.Application/MyProject.Application.csproj` — 新增 Volo.Abp.Ddd.Application、Volo.Abp.Authorization 包引用，FrameworkReference Microsoft.AspNetCore.App
- `src/MyProject.Application/MyProjectApplicationAutoMapperProfile.cs` — 新增所有实体→DTO 映射
- `src/MyProject.Domain.Shared/Localization/MyProject/zh-Hans.json` — 新增权限本地化文本
- `src/MyProject.Domain.Shared/Localization/MyProject/en.json` — 新增权限本地化文本

新增文件（第 2 步）：

- `src/MyProject.Domain.Shared/Enums/ProductionOrderStatus.cs` — 生产工单状态枚举
- `src/MyProject.Domain.Shared/Enums/ProductionOrderOperationStatus.cs` — 工序明细行状态枚举
- `src/MyProject.Domain.Shared/Enums/RouteChangeSource.cs` — 变更来源枚举
- `src/MyProject.Domain/Entities/ProductionOrder.cs` — 生产工单聚合根
- `src/MyProject.Domain/Entities/ProductionOrderOperation.cs` — 工序明细行子实体
- `src/MyProject.Domain/Entities/ProductionOrderRouteChangeLog.cs` — 工艺路线变更日志

新增文件（第 3 步）：

- `src/MyProject.EntityFrameworkCore/Migrations/20260722132523_AddProductionOrderEntities.cs` — 迁移文件（表名使用 App 前缀，符合 ABP 规范）
- `src/MyProject.EntityFrameworkCore/Migrations/20260722132523_AddProductionOrderEntities.Designer.cs` — 迁移设计器文件
- `src/MyProject.EntityFrameworkCore/Migrations/MyProjectDbContextModelSnapshot.cs` — 模型快照（已更新）

新增文件（第 4 步 - 主数据实体）：

- `src/MyProject.Domain/Entities/Product.cs` — 产品主数据
- `src/MyProject.Domain/Entities/ProductionOrganization.cs` — 生产组织主数据
- `src/MyProject.Domain/Entities/ProcessRoute.cs` — 工艺路线主数据
- `src/MyProject.Domain/Entities/ProcessRouteOperation.cs` — 工艺路线工序子实体

新增文件（第 4 步 - 领域服务）：

- `src/MyProject.Domain/Services/IOrderNumberGenerator.cs` — 工单号生成器接口
- `src/MyProject.Domain/Services/OrderNumberGenerator.cs` — 工单号生成器实现（PO+yyyyMMdd+4位流水）
- `src/MyProject.Domain/Services/ProductionOrderManager.cs` — 建单领域服务

新增文件（第 4 步 - 迁移）：

- `src/MyProject.EntityFrameworkCore/Migrations/20260722134540_AddMasterDataEntities.cs` — 主数据表迁移
- `src/MyProject.EntityFrameworkCore/Migrations/20260722134540_AddMasterDataEntities.Designer.cs` — 迁移设计器

新增文件（第 5 步 - 权限）：

- `src/MyProject.Application.Contracts/Permissions/MyProjectPermissions.cs` — 权限名称常量（生产工单、产品、生产组织、工艺路线）
- `src/MyProject.Application.Contracts/Permissions/MyProjectPermissionDefinitionProvider.cs` — 权限定义提供者

新增文件（第 5 步 - DTO）：

- `src/MyProject.Application.Contracts/Dtos/ProductionOrderDto.cs` — 生产工单输出 DTO + 工序明细输出 DTO
- `src/MyProject.Application.Contracts/Dtos/CreateProductionOrderDto.cs` — 手工创建输入 DTO
- `src/MyProject.Application.Contracts/Dtos/CreateProductionOrderFromErpDto.cs` — ERP 接口创建输入 DTO
- `src/MyProject.Application.Contracts/Dtos/GetProductionOrderListDto.cs` — 生产工单列表查询 DTO
- `src/MyProject.Application.Contracts/Dtos/ProductDtos.cs` — 产品 DTO + CreateUpdateProductDto
- `src/MyProject.Application.Contracts/Dtos/ProductionOrganizationDtos.cs` — 生产组织 DTO + CreateUpdateDto
- `src/MyProject.Application.Contracts/Dtos/ProcessRouteDtos.cs` — 工艺路线 DTO + CreateUpdateDto + 工序 DTO

新增文件（第 5 步 - IAppService 接口）：

- `src/MyProject.Application.Contracts/IProductionOrderAppService.cs` — 生产工单应用服务接口（自定义，含 Create/CreateFromErp/Get/GetList）
- `src/MyProject.Application.Contracts/IProductAppService.cs` — 产品应用服务接口（继承 ICrudAppService）
- `src/MyProject.Application.Contracts/IProductionOrganizationAppService.cs` — 生产组织应用服务接口（继承 ICrudAppService）
- `src/MyProject.Application.Contracts/IProcessRouteAppService.cs` — 工艺路线应用服务接口（继承 ICrudAppService）

新增文件（第 5 步 - AppService 实现）：

- `src/MyProject.Application/ProductionOrderAppService.cs` — 生产工单应用服务（调用领域服务，[Authorize] 权限控制）
- `src/MyProject.Application/ProductAppService.cs` — 产品应用服务（继承 CrudAppService，权限策略名）
- `src/MyProject.Application/ProductionOrganizationAppService.cs` — 生产组织应用服务（继承 CrudAppService）
- `src/MyProject.Application/ProcessRouteAppService.cs` — 工艺路线应用服务（继承 CrudAppService，重写 Create/Update 处理工序子集合）

未修改：

- HttpApi 层。
- 测试项目。

后续每次编码后，必须按以下模板追加记录：

```md
#### 第 N 次代码改动记录

本次目标：

用户确认内容：

新增文件：

修改文件：

删除文件：

本次未做：

不确定点：
```

### 11.4 文件改动清单

| 文件路径 | 状态 | 说明 | 是否已测试 |
|---|---|---|---|
| `mes-workorder-context.md` | 已修改 | 记录 MES 工单模块需求、ABP 实施指引和交接规则 | 不适用 |
| `src/MyProject.Domain.Shared/Enums/ProductionOrderStatus.cs` | 新增 | 生产工单状态枚举 | 编译通过 |
| `src/MyProject.Domain.Shared/Enums/ProductionOrderOperationStatus.cs` | 新增 | 工序明细行状态枚举 | 编译通过 |
| `src/MyProject.Domain.Shared/Enums/RouteChangeSource.cs` | 新增 | 变更来源枚举 | 编译通过 |
| `src/MyProject.Domain/Entities/ProductionOrder.cs` | 新增 | 生产工单聚合根 | 编译通过 |
| `src/MyProject.Domain/Entities/ProductionOrderOperation.cs` | 新增 | 工序明细行子实体 | 编译通过 |
| `src/MyProject.Domain/Entities/ProductionOrderRouteChangeLog.cs` | 新增 | 工艺路线变更日志 | 编译通过 |
| `src/MyProject.EntityFrameworkCore/EntityFrameworkCore/MyProjectDbContext.cs` | 已修改 | 新增 DbSet 和实体映射配置 | 编译通过 |
| `src/MyProject.EntityFrameworkCore/Migrations/20260722132523_AddProductionOrderEntities.cs` | 新增 | 生产工单表迁移（App前缀，ABP规范） | 数据库已更新 |
| `src/MyProject.EntityFrameworkCore/Migrations/20260722132523_AddProductionOrderEntities.Designer.cs` | 新增 | 迁移设计器 | 不适用 |
| `src/MyProject.EntityFrameworkCore/Migrations/MyProjectDbContextModelSnapshot.cs` | 已修改 | 模型快照更新 | 不适用 |
| `src/MyProject.Domain/Entities/Product.cs` | 新增 | 产品主数据 | 编译通过 |
| `src/MyProject.Domain/Entities/ProductionOrganization.cs` | 新增 | 生产组织主数据 | 编译通过 |
| `src/MyProject.Domain/Entities/ProcessRoute.cs` | 新增 | 工艺路线主数据 | 编译通过 |
| `src/MyProject.Domain/Entities/ProcessRouteOperation.cs` | 新增 | 工艺路线工序子实体 | 编译通过 |
| `src/MyProject.Domain/Services/IOrderNumberGenerator.cs` | 新增 | 工单号生成器接口 | 编译通过 |
| `src/MyProject.Domain/Services/OrderNumberGenerator.cs` | 新增 | 工单号生成器实现 | 编译通过 |
| `src/MyProject.Domain/Services/ProductionOrderManager.cs` | 新增 | 建单领域服务 | 编译通过 |
| `src/MyProject.EntityFrameworkCore/Migrations/20260722134540_AddMasterDataEntities.cs` | 新增 | 主数据表迁移 | 数据库已更新 |
| `src/MyProject.EntityFrameworkCore/Migrations/20260722134540_AddMasterDataEntities.Designer.cs` | 新增 | 迁移设计器 | 不适用 |
| `src/MyProject.Application.Contracts/Permissions/MyProjectPermissions.cs` | 新增 | 权限名称常量 | 编译通过 |
| `src/MyProject.Application.Contracts/Permissions/MyProjectPermissionDefinitionProvider.cs` | 新增 | 权限定义提供者 | 编译通过 |
| `src/MyProject.Application.Contracts/Dtos/ProductionOrderDto.cs` | 新增 | 生产工单+工序输出DTO | 编译通过 |
| `src/MyProject.Application.Contracts/Dtos/CreateProductionOrderDto.cs` | 新增 | 手工创建输入DTO | 编译通过 |
| `src/MyProject.Application.Contracts/Dtos/CreateProductionOrderFromErpDto.cs` | 新增 | ERP创建输入DTO | 编译通过 |
| `src/MyProject.Application.Contracts/Dtos/GetProductionOrderListDto.cs` | 新增 | 列表查询DTO | 编译通过 |
| `src/MyProject.Application.Contracts/Dtos/ProductDtos.cs` | 新增 | 产品DTO+CreateUpdate | 编译通过 |
| `src/MyProject.Application.Contracts/Dtos/ProductionOrganizationDtos.cs` | 新增 | 生产组织DTO+CreateUpdate | 编译通过 |
| `src/MyProject.Application.Contracts/Dtos/ProcessRouteDtos.cs` | 新增 | 工艺路线DTO+CreateUpdate+工序 | 编译通过 |
| `src/MyProject.Application.Contracts/IProductionOrderAppService.cs` | 新增 | 生产工单应用服务接口 | 编译通过 |
| `src/MyProject.Application.Contracts/IProductAppService.cs` | 新增 | 产品应用服务接口(ICrudAppService) | 编译通过 |
| `src/MyProject.Application.Contracts/IProductionOrganizationAppService.cs` | 新增 | 生产组织应用服务接口 | 编译通过 |
| `src/MyProject.Application.Contracts/IProcessRouteAppService.cs` | 新增 | 工艺路线应用服务接口 | 编译通过 |
| `src/MyProject.Application/ProductionOrderAppService.cs` | 新增 | 生产工单应用服务实现 | 编译通过 |
| `src/MyProject.Application/ProductAppService.cs` | 新增 | 产品应用服务(CrudAppService) | 编译通过 |
| `src/MyProject.Application/ProductionOrganizationAppService.cs` | 新增 | 生产组织应用服务(CrudAppService) | 编译通过 |
| `src/MyProject.Application/ProcessRouteAppService.cs` | 新增 | 工艺路线应用服务(CrudAppService) | 编译通过 |
| `src/MyProject.Application/MyProjectApplicationAutoMapperProfile.cs` | 已修改 | 新增实体→DTO映射 | 编译通过 |
| `src/MyProject.Application.Contracts/MyProject.Application.Contracts.csproj` | 已修改 | 新增Ddd.Application.Contracts包引用 | 编译通过 |
| `src/MyProject.Application/MyProject.Application.csproj` | 已修改 | 新增Ddd.Application+Authorization包+FrameworkReference | 编译通过 |
| `src/MyProject.Domain.Shared/Localization/MyProject/zh-Hans.json` | 已修改 | 新增权限本地化 | 编译通过 |
| `src/MyProject.Domain.Shared/Localization/MyProject/en.json` | 已修改 | 新增权限本地化 | 编译通过 |

后续每次新增、修改、删除业务代码文件后，必须同步更新此表。

### 11.5 当前阻塞问题

无阻塞性问题。

### 11.5.1 技术债务与待改进项

1. **Product/ProductionOrganization 实体缺少 Update 方法**：当前 Update 操作在 AppService 中直接 new 新实体替换，不符合 DDD 规范（应通过实体方法修改字段）。首版暂时可用，后续需补充实体 Update 方法。
2. **ProcessRoute Update 同样存在重建问题**：ProcessRouteAppService.UpdateAsync 中清空 Operations 再重建，而非通过实体方法操作集合，不符合 DDD 规范。
3. **OrderNumberGenerator 并发安全**：当前实现基于查询当日最大号递增，高并发下可能产生重复号。首版可接受，后续需加唯一约束或分布式锁。
4. **ProductionOrderAppService.GetListAsync 未 Include Operations**：列表查询未加载工序明细行，ProductionOrderDto 中无 Operations 集合字段。如需列表中展示工序信息，需后续补充。
5. **ProductionOrderDto 缺少 Operations 集合**：输出 DTO 中未包含工序明细行集合，需后续补充。
6. **CrudAppService 基类与自定义接口混用**：主数据用 CrudAppService（权限通过 PolicyName 控制），生产工单用自定义接口（权限通过 [Authorize] 特性控制）。两种风格并存，后续可考虑统一。
7. **MyProject.Application.csproj 新增了 FrameworkReference Microsoft.AspNetCore.App**：这使 Application 层依赖 ASP.NET Core，虽然功能正常但不符合 ABP 理想分层（Application 层应可脱离 Web 运行）。ABP 官方模板中 Application 层通过 Volo.Abp.Authorization 包间接引用。当前为编译通过临时方案，后续可考虑移除 FrameworkReference 改用更轻量的引用方式。

### 11.6 测试与验证记录

第 2 步至第 5 步完成后已执行编译验证和数据库更新。

已执行：

- 文档结构检查。
- 旧术语残留检查。
- ABP 实施指引章节完整性检查。
- `dotnet build`：编译通过，零错误。
- `dotnet ef migrations add AddProductionOrderEntities`：生产工单表迁移生成成功。
- `dotnet ef database update`：生产工单表数据库更新成功。
- `dotnet ef migrations add AddMasterDataEntities`：主数据表迁移生成成功。
- `dotnet ef database update`：主数据表数据库更新成功。

未执行：

- 单元测试
- 集成测试

后续每次编码后，必须按以下模板追加测试记录：

```md
#### YYYY-MM-DD 测试记录

执行命令：

结果：

如果失败，错误摘要：

处理状态：

下一步：
```

### 11.7 用户已确认事项快照

1. 当前只做后端，不做前端。
2. 首版只做生产工单。
3. 不设置独立工序工单单据。
4. 工序以生产工单工序明细行存在。
5. "分解"指在生产工单内生成工序明细行。
6. 建单成功后状态为已下达。
7. ERP 来单默认已下达。
8. 创建时自动匹配工艺路线，匹配不到仍允许建单。
9. 分解生成工序明细行前必须补齐并确认工艺路线。
10. 分解后工艺路线锁定。
11. 修改路线需先删除未开工的工序明细行。
12. 手工和 ERP 创建共用同一个建单领域服务。
13. 产品编码和生产组织做存在性校验，ERP 来单同样严格校验。
14. ERP 源单据 id 非空唯一，空值不参与唯一约束。
15. 接口系统标识、RequestId 不进入生产工单业务表。
16. 除本文档外，任何代码修改都必须先得到用户确认。
17. 后续 AI 每一步都必须先向用户确认。
18. 用户每次会提供最新代码给 AI 读取，不需要记录版本控制信息。
19. 不新建独立 ABP 模块，在现有项目内用目录/命名空间组织。
20. 去掉 Mes 前缀：表名、类名、目录/命名空间都不用 Mes。
21. 枚举命名去掉 Work：ProductionOrderStatus、ProductionOrderOperationStatus。
22. 首批实体确认：ProductionOrder、ProductionOrderOperation、ProductionOrderRouteChangeLog。
23. 数据库为 MySQL（Pomelo 9.0.0），连接 localhost:3306/MySystem。
24. 严格按照 ABP 框架规范执行，不确定时必须先问用户。
25. 业务表名使用 App 前缀（MyProjectConsts.DbTablePrefix = "App"），表名为 AppProductionOrders、AppProductionOrderOperations、AppProductionOrderRouteChangeLogs。
26. 主数据首版先做最小模型（Product、ProductionOrganization、ProcessRoute + ProcessRouteOperation），包含基础字段。
27. 单位首版只做必填校验，不校验计量单位主数据。
28. 工单号格式：PO + yyyyMMdd + 4位流水号（如 PO202607220001），基于当日最大号递增。
29. Guid 为主键（内码），业务编码（如产品编码、工艺路线号）为编码，两者分离。
30. 手工和ERP创建放同一个 AppService，通过不同方法区分入口。
31. 需要 ABP 权限控制。
32. 主数据需要 CRUD 应用服务。
33. 主数据 AppService 继承 ABP 标准 CrudAppService，接口继承 ICrudAppService（ABP 9.3.0 无 IAsyncCrudAppService，只有 ICrudAppService）。
34. [Authorize] 属性来自 Microsoft.AspNetCore.Authorization，Application 项目需 FrameworkReference Microsoft.AspNetCore.App。

### 11.8 当前待用户确认问题

1. ~~产品、生产组织、工艺路线主数据如果不存在，首版如何处理。~~ 已确认：先做最小主数据模型。
2. ~~单位是否只做必填，还是校验计量单位主数据。~~ 已确认：只做必填。
3. ~~工单号生成规则。~~ 已确认：PO+yyyyMMdd+4位流水。
4. ~~枚举数据库存储方式（int 还是 string）。~~ 已确认：int。
5. ~~ERP 源单据 id 唯一索引实现方式（MySQL 下）。~~ 已确认：MySQL UNIQUE 索引天然允许多个 NULL。
6. 接口平台日志是否首版落库。
7. 首版测试范围：单元测试、应用服务测试、集成测试分别做到什么程度。
8. ~~是否开始第 5 步：应用服务与 DTO。~~ 已完成。

### 11.9 下一位 AI 接手时必须先做的事

接手后必须按顺序执行：

1. 阅读本文档，重点阅读第 4、5、10、11 节。
2. 扫描用户提供的最新代码目录。
3. 核对本文档记录的实施状态是否与最新代码一致。
4. 如果一致，继续当前步骤。
5. 如果不一致，暂停并向用户说明差异。
6. 未经用户确认，不得修改任何业务代码。
7. 当前建议下一步是执行第 6 步：测试。

### 11.10 下一位 AI 建议开场白

```text
我已阅读 mes-workorder-context.md。当前 MES 工单模块第 5 步（应用服务与 DTO）已完成。数据库中已有 7 张业务表，领域服务和应用服务层已就绪。权限定义（4 组权限）、DTO（7 个文件）、IAppService 接口（4 个）、AppService 实现（4 个，主数据用 CrudAppService 基类）、AutoMapper 映射已配置完成。编译通过，0 错误。下一步建议执行第 6 步：测试。请确认测试范围。
```

### 11.11 暂停前交接模板

每次 AI coding 暂停前，必须按以下模板更新本节：

```md
#### 暂停交接记录：YYYY-MM-DD HH:mm

本次开始时所在步骤：

本次目标：

用户本次确认内容：

本次完成内容：

本次新增文件：

本次修改文件：

本次删除文件：

本次未完成内容：

当前错误 / 风险 / 不确定点：

已执行测试：

测试结果：

下一步建议：

下一位 AI 接手后的第一句话：
```

#### 暂停交接记录：2026-07-22 23:30

本次开始时所在步骤：第 0 步（项目结构扫描）

本次目标：全面接手项目，验证文档与代码一致性，继续推进实施

用户本次确认内容：
- 不新建独立 ABP 模块，去掉 Mes 前缀
- 枚举命名去掉 Work
- 主数据先做最小模型
- 单位只做必填
- 工单号格式 PO+yyyyMMdd+4位流水
- 手工和ERP放同一个 AppService
- 需要权限控制
- 主数据需要 CRUD
- 严格按照 ABP 规范

本次完成内容：
- 第 0 步：扫描项目结构（已有 ABP 骨架，无 MES 业务代码）
- 第 1 步：确认模块与命名空间方案
- 第 2 步：领域模型最小闭环（3 枚举 + 3 实体）
- 第 3 步：EF Core 映射与迁移（7 张业务表，App 前缀）
- 第 4 步：建单领域服务 + 最小主数据（4 主数据实体 + 领域服务 + 工单号生成器）
- 第 5 步：应用服务与 DTO（权限 + DTO + IAppService + AppService + AutoMapper）
- 修正：文件名 ProductDto.cs → ProductionOrderDto.cs

本次新增文件：见 11.3 节和 11.4 节完整清单

本次修改文件：见 11.3 节和 11.4 节完整清单

本次删除文件：无

本次未完成内容：
- 第 6 步：测试
- 分解逻辑
- 状态联动逻辑
- 工艺路线修改日志记录
- 工艺路线版本链"保存即结算"

当前错误 / 风险 / 不确定点：
- 无编译错误
- 技术债务见 11.5.1 节（7 项）
- 接口平台日志首版是否落库未确认

已执行测试：编译验证（0 错误）

测试结果：编译通过

下一步建议：执行第 6 步测试，需用户确认测试范围（建议先做单元测试+应用服务测试，集成测试后续再加）

下一位 AI 接手后的第一句话：我已阅读 mes-workorder-context.md。当前 MES 工单模块第 5 步（应用服务与 DTO）已完成，编译通过。下一步是第 6 步测试，需确认测试范围。请确认是否开始。

## 12. 后续执行原则

1. 下一步不是直接编码，而是进入编码前落地确认。
2. 推荐先执行“第 0 步：扫描项目结构”，然后向用户汇报扫描结果。
3. 推荐再确认模块/实体划分与开发顺序。
4. 得到用户明确同意后，才开始修改业务代码。
5. 如果用户指定其他方向，则优先按用户指定方向推进。
6. 每一步开始前必须向用户确认，每一步完成后必须汇报改动、测试方法、不确定点和下一步建议。
