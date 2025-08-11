使用方法：
	八叉树空间不是动态的，是静态由自定义划分的，避免创建八叉树的时候纳入的物体太少，或者新创建的物体在八叉树之外不得不重新创建八叉树
	需要一开始世界要起码存在两个物体(带有碰撞体),加入到OctreeGenerator脚本的公开列表中，这是用来创建八叉树的起始空间的
	在inspector窗口中进行引用
	每个一开始就存在的需要纳入八叉树的物体都要挂载OctreeMono脚本
	需要一个不变的物体来挂载OctreeGenerator脚本，来创建八叉树
	然后需要进行空间划分的需要获取自身的OctreeMono脚本，调用public List<GameObject> GetCheckTargetList()方法

	空间划分的使用：
		需要得到自身对象的OctreeMono脚本，调用ProvideTargetPosition(UnityAction<GameObject> action)方法
		传入一个有参无返回值的委托，装载的是筛选条件，经过条件筛选的物体才会被返回




代码思路：
	八叉树创建逻辑：
		Octreegenerator的一个公开列表存储至少两个物体，用于定义起始最大八叉树空间，这表明八叉树的最大空间是静态的
		传递调用Octree构造函数，传递进去，创建根节点
		成功创建八叉树


	物体添加到八叉树逻辑：
		当物体新创建的时候，在OctreeMono脚本的awake函数就会创建对应的OctreeObject并加入到GameDataMgr中的一个哈希表
		这个哈希表被Octree获取，会在生命周期函数中执行方法，持续把这个哈希表中的元素加入八叉树
		解决物体的添加和动态添加


	空间划分检测逻辑：
		空间划分的主要实现在物体的OctreeMono脚本中：
		更新节点:
		实现一个hashset字段，来存储OctreeNode
		主要依靠CheckPositionChange()方法，当物体位置发生变动的时候才调用UpdateSurroundNode()方法，更新存储的节点
		以及在update中调用方法CheckSurroundNodeList()，来持续检测节点表中存在节点变成非叶子节点
		若检测到非叶子节点，则也调用UpdateSurroundNode()方法来更新节点表
		在update中CheckPositionChange()方法应该要在CheckSurroundNodeList()方法前执行，免得位置发生改变后，更新两次节点表

		使用空间检测得到对象：
		需要得到自身对象的OctreeMono脚本，调用ProvideTargetPosition(UnityAction<GameObject> action)方法
		传入一个有参无返回值的委托，装载的是筛选条件，经过条件筛选的物体才会被返回
		然后方法把存储的相交的节点的存储的OctreeObject（插入八叉树的对象）遍历出来并进行计算距离，然后使用优先队列进行排序，返回最近的一位

		为何要只更新节点，而不顺便把节点的子物体一起更新？
		物体调用逻辑是这样的，没有目标的时候就一直请求，当有目标的时候就不再请求了，此时就没必要更新子物体表


		

注意事项：
	所有需要包含被八叉树管理的物体都要有碰撞体，便于生成包围盒，因为计算包围盒是根据collider来的
	即使物体挂载的是CharacterController组件也没事，因为跟capsuleCollider一样都是特殊的collider，直接collider就可以得到
	所有挂载OctreeMono脚本的对象的主脚本都需要实现一个IGetFloat接口让脚本能获取到攻击范围




介绍

	允许物体的OctreeObject存储在多个节点，允许物体存储在多个节点中以保证空间完整性，也就是，物体包围盒如果碰到两个八叉树的子空间，就都会存储在他们的子节点中
	这个系统是只有叶子节点才存储值的，所以还需要一个bool值方便快速判断


	动态更新
		
		OctreeObject类存储记录物体所属的所有节点
		在八叉树的动态更新中，移除旧关联时不需要删除旧节点。节点的生命周期与空间划分强相关，而非单纯依赖是否包含物体。
		然后重新在根节点中调用插入方法
		具体地来说：
		移动更新：在OctreeMono脚本中进行位置检测，同步更新bounds位置，如果检测出发生了移动，则通过脚本中的OctreeObject引用调用其中的更新方法OctreeUpdate()
		OctreeObject的更新方法又会把自己传进八叉树的ReDivide(OctreeObject @object)方法，就会把这个OctreeObject加入到一个列表中
		而Octree中public void Redivide()方法在构造函数的时候就通过Mono模块注册到fixUpdate的事件（无深意，这里只是为了减少更新频率）中，每次列表不为空就会执行该方法
		这个方法会把列表中的物体循环从根节点中插入
		这样如果有大量物体进行移动，同一移动到某一生命周期函数统一处理更新，避免一个更新一次

		物体添加更新：
			当物体新创建的时候，在OctreeMono脚本的awake函数就会创建对应的OctreeObject并加入到GameDataMgr中的一个哈希表
			这个哈希表被Octree获取，会在生命周期函数中进行方法，持续把这个哈希表中的元素加入八叉树



类Octree：
	
	构造函数
	public Octree(OctreeMono[] worldSpace, float minNodeSize)
		调用CalculateBounds函数进行计算包围盒。
		调用 CreateTree创建根节点

	private void CreateTree(float minNodeSize)
		创建OctreeNode根节点root

	private void CalculateBounds(List<GameObject> worldSpace)
		计算实现容纳所有世界物体中的包围盒（根据物体的collider）

	public void ReDivide(OctreeObject object)
		OctreeObject调用，一般是节点重新插入的时候会使用，用于把节点添加到持续插入的哈希表中，等待插入；这里并不直接插入，等到Redivide()被调用时统一插入

	public void Redivide()
		在构造函数中注册到Mono模块中的更新阶段的生命周期函数
		会一直执行把GamedataMgr中的对应哈希表中的OctreeObject插入八叉树




类OctreeGenerator
	创建一个八叉树Octree，需要继承mono
	调用绘制方法


类OctreeObject
	
	OctreeObject 类是 八叉树（Octree）系统中 “单个可管理物体” 的封装层，负责抽象物体的空间信息（包围盒）并提供相交检测能力。

	OctreeObject 是 八叉树与实际物体之间的 “翻译层”：
	把 Unity 物体的空间信息抽象为通用的 Bounds，并提供标准化的相交检测方法，让八叉树能高效管理 3D 空间中的物体。
	OctreeObject 的核心作用是抽象 “空间物体” 的通用属性，与 Unity 引擎解耦：
	八叉树的核心逻辑（插入、更新、查询）本质是 “处理空间中的物体”，不需要依赖 Unity 的GameObject、Collider等类型
	解耦：八叉树逻辑（空间划分）与 Unity 引擎 API（GameObject、Collider）解耦，便于移植或扩展。
	复用：OctreeObject 可扩展更多属性（如物体 ID、自定义数据），而不影响八叉树核心逻辑
	如果直接用OctreeMono作为八叉树的处理对象，八叉树核心会强依赖 Unity 引擎

	public bool Intersects(Bounds boundsToCheck)
		当八叉树节点分割空间后，需判断物体是否与子节点的包围盒相交，从而决定物体归属。
		方法用来检测boundsToCheck和该OctreeObject物体的包围盒是否相交



类OctreeNode
	OctreeNode 是 八叉树（Octree）的核心节点类，负责定义单个节点的空间范围、子节点结构，以及管理所属物体。

	构造函数
	public OctreeNode(Bounds bounds, float minSize)
		用于将父节点的给予的空间进行分割，分割成八块，并进行存储
		当需要再继续分割的时候，就new八个节点出来，把这八个bounds分别赋予八个节点，让他们的构造函数进行分割

类OctreeMono
	所有需要纳入八叉树包围盒的物体都需要挂载这个脚本，用于物体和Octree系统交流
	OctreeMono 的定位是 “Unity 桥梁”，负责衔接 Unity 物体（MonoBehaviour）与八叉树系统

	Init方法
		通过物体的Coliider组件得到物体的包围盒
		如果该物体是需要检测外界的距离的，需要利用八叉树的空间划分的物体


	public List<GameObject> GetCheckTargetList()
		通过脚本中获取的OctreeObject引用来调用八叉树的RoomSearch方法来得到


		实现性能优化：动态物体就是持续获取，持续访问八叉树系统；静态物体则进行有条件更新，一开始只获取一次，在每次使用的时候都检查节点是否是叶子节点，如果不是才再次获取