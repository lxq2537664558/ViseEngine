<GameData.Skill.SkillTemplate Ver="0">
	<SkillId Type="mscorlib.dll@System.UInt16" Value="102"/>
	<SkillType Type="game@GameData.Skill.ESkillType" Value="InitiativeSkill"/>
	<SkillAttackType Type="game@GameData.Skill.ESkillAttackType" Value="Summon"/>
	<AttackAction Type="mscorlib.dll@System.String" Value="StayAttack"/>
	<AttackRadius Type="mscorlib.dll@System.Single" Value="10"/>
	<AttackNotifyEffects Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.SkillTemplate+NotifyEffect]]">
		<Data Type="game@GameData.Skill.SkillTemplate+NotifyEffect" Value="null">
			<NotifyName Type="mscorlib.dll@System.String" Value="Effect"/>
			<SocketEffects Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.SkillTemplate+SocketEffect]]">
				<Data Type="game@GameData.Skill.SkillTemplate+SocketEffect" Value="null">
					<SocketName Type="mscorlib.dll@System.String" Value="hp_socket_attack"/>
					<Effects Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.SkillTemplate+Effect]]">
						<Data Type="game@GameData.Skill.SkillTemplate+Effect" Value="null">
							<EffectId Type="mscorlib.dll@System.Guid" Value="804064bb-3a19-4fa5-b55b-23846eabf30b"/>
							<Duration Type="mscorlib.dll@System.Int64" Value="500"/>
							<Pos Type="cscommon@SlimDX.Vector3" Value="0,0,0"/>
							<Scale Type="mscorlib.dll@System.Single" Value="1"/>
							<Rotate Type="cscommon@SlimDX.Vector3" Value="0,0,0"/>
							<InheritRotate Type="mscorlib.dll@System.Boolean" Value="True"/>
							<InheritRotateWhenBorn Type="mscorlib.dll@System.Boolean" Value="False"/>
							<ChainType Type="game@GameData.Skill.EEffectChainType" Value="Line"/>
							<ChainId Type="mscorlib.dll@System.Guid" Value="00000000-0000-0000-0000-000000000000"/>
							<TrailPoolSize Type="mscorlib.dll@System.Int32" Value="100"/>
							<TrailWidth Type="mscorlib.dll@System.Single" Value="0.5"/>
							<ChainDuration Type="mscorlib.dll@System.Int64" Value="9223372036854775807"/>
							<ChainSegmentDuration Type="mscorlib.dll@System.Int64" Value="9223372036854775807"/>
							<SegmentMinDistance Type="mscorlib.dll@System.Single" Value="0.1"/>
							<TopInLinePercent Type="mscorlib.dll@System.Single" Value="0.3"/>
							<TopHeight Type="mscorlib.dll@System.Single" Value="3"/>
							<TopWidth Type="mscorlib.dll@System.Single" Value="0"/>
							<RanTopInLineMin Type="mscorlib.dll@System.Single" Value="-1"/>
							<RanTopHeightMin Type="mscorlib.dll@System.Single" Value="3.402823E+38"/>
							<RanTopWidthMin Type="mscorlib.dll@System.Single" Value="3.402823E+38"/>
						</Data>
					</Effects>
				</Data>
			</SocketEffects>
			<ShakeName Type="mscorlib.dll@System.String" Value=""/>
		</Data>
	</AttackNotifyEffects>
	<ThrowRoleId Type="mscorlib.dll@System.UInt16" Value="20000"/>
	<HalfHeight Type="mscorlib.dll@System.Single" Value="2"/>
	<HeightOffset Type="mscorlib.dll@System.Single" Value="0"/>
	<Radius Type="mscorlib.dll@System.Single" Value="1"/>
	<ThrowRoleSpeed Type="mscorlib.dll@System.Single" Value="12"/>
	<AIGuid Type="mscorlib.dll@System.Guid" Value="b0fab31b-6b52-4848-a2cc-585da5c1adda"/>
	<LiveTime Type="mscorlib.dll@System.Single" Value="1.2"/>
	<SkillLevelDatas Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.SkillTemplate+SkillLevelParam]]">
		<Data Type="game@GameData.Skill.SkillTemplate+SkillLevelParam" Value="null">
			<SkillLevel Type="mscorlib.dll@System.Byte" Value="0"/>
			<SkillDescription Type="mscorlib.dll@System.String" Value=""/>
			<CD Type="mscorlib.dll@System.Single" Value="0"/>
			<SkillConsumeMP Type="mscorlib.dll@System.UInt16" Value="0"/>
			<LevelupNeedRoleLevel Type="mscorlib.dll@System.SByte" Value="0"/>
			<RoleDamageRate Type="mscorlib.dll@System.Single" Value="0"/>
			<DamageAdd Type="mscorlib.dll@System.Int32" Value="0"/>
			<Buffs Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.BuffParam]]"/>
		</Data>
	</SkillLevelDatas>
	<SkillName Type="mscorlib.dll@System.String" Value=""/>
	<FireSkill Type="game@GameData.Skill.FireSkillType" Value="Prompt"/>
	<FireSkillTime Type="mscorlib.dll@System.Int32" Value="0"/>
	<SkillActionLoop Type="mscorlib.dll@System.Boolean" Value="False"/>
	<ControlLife Type="mscorlib.dll@System.UInt32" Value="360"/>
	<SkillIcon Type="mscorlib.dll@System.Guid" Value="00000000-0000-0000-0000-000000000000"/>
	<SkillPressIcon Type="mscorlib.dll@System.Guid" Value="00000000-0000-0000-0000-000000000000"/>
	<SkillDisableIcon Type="mscorlib.dll@System.Guid" Value="00000000-0000-0000-0000-000000000000"/>
	<SkillSumType Type="game@GameData.Skill.ESkillSumType" Value="CommonSum"/>
	<SkillRangeType Type="game@GameData.Skill.ESkillRangeType" Value="One"/>
	<SkillshotMaterial Type="mscorlib.dll@System.Guid" Value="da50c1d6-b4e2-41b2-a4e3-817aef2f0224"/>
	<OffsetType Type="game@GameData.Skill.EOffsetType" Value="None"/>
	<ChannelAction Type="mscorlib.dll@System.String" Value=""/>
	<AttackSoundId Type="mscorlib.dll@System.UInt16" Value="65535"/>
	<RotationSkillTime Type="mscorlib.dll@System.Int32" Value="0"/>
	<LoopAction Type="mscorlib.dll@System.String" Value=""/>
	<EndAction Type="mscorlib.dll@System.String" Value=""/>
	<Force2BeAttack Type="mscorlib.dll@System.Boolean" Value="False"/>
	<BeAttackAction Type="mscorlib.dll@System.String" Value="BeAttack"/>
	<LoopBeAttackAction Type="mscorlib.dll@System.String" Value=""/>
	<EndBeAttackAction Type="mscorlib.dll@System.String" Value=""/>
	<BeAttackLoop Type="mscorlib.dll@System.Boolean" Value="False"/>
	<BeAttackPlayRate Type="mscorlib.dll@System.Single" Value="1"/>
	<ChannelDuration Type="mscorlib.dll@System.Int64" Value="3000"/>
	<CanBeBreak Type="mscorlib.dll@System.Boolean" Value="False"/>
	<JumpUpHeight Type="mscorlib.dll@System.Single" Value="1"/>
	<JumpUpTime Type="mscorlib.dll@System.Int32" Value="595"/>
	<MoveTime Type="mscorlib.dll@System.Int32" Value="595"/>
	<ChannelNotifyEffects Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.SkillTemplate+NotifyEffect]]"/>
	<AttackEffectRepeatAdd Type="mscorlib.dll@System.Boolean" Value="False"/>
	<BeAttackNotifyEffects Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.SkillTemplate+NotifyEffect]]"/>
	<ChainNotifyPointName Type="mscorlib.dll@System.String" Value=""/>
	<ChainSocketName Type="mscorlib.dll@System.String" Value="HP_FixSocket_gethit_effect"/>
	<RemoveSocketName Type="mscorlib.dll@System.String" Value=""/>
	<NotifyShakes Type="mscorlib.dll@System.Collections.Generic.List`1[[game@GameData.Skill.SkillTemplate+NotifyShake]]"/>
	<SkillTarget Type="game@GameData.Skill.SkillTarget" Value="Other"/>
	<SkillTargetNumber Type="mscorlib.dll@System.Int32" Value="1"/>
	<SkillOperation Type="game@GameData.Skill.ESkillOperationType" Value="NonSkillshot"/>
	<ReverseShoot Type="mscorlib.dll@System.Boolean" Value="False"/>
	<RandomRadius Type="mscorlib.dll@System.Single" Value="12"/>
	<TargetState Type="game@GameData.Skill.TargetState" Value="None"/>
	<SkillProbability Type="mscorlib.dll@System.UInt32" Value="0"/>
	<C2SDistanceError Type="mscorlib.dll@System.Single" Value="5"/>
	<SummonSkillType Type="game@GameData.Skill.ESkillSummonType" Value="Ball"/>
	<SummonBindRole Type="mscorlib.dll@System.Boolean" Value="False"/>
	<ThrowRoleCount Type="mscorlib.dll@System.UInt16" Value="1"/>
	<ThrowRoleRate Type="mscorlib.dll@System.Single" Value="1"/>
	<GameType Type="cscommon@CSUtility.Component.EActorGameType" Value="EffectNpc"/>
	<SummonOffsetType Type="game@GameData.Skill.EOffsetType" Value="None"/>
	<SummonOffsetSpeed Type="mscorlib.dll@System.Single" Value="1"/>
	<EmissionType Type="game@GameData.Skill.EEmissionType" Value="Distribute"/>
	<EmissionAngle Type="mscorlib.dll@System.UInt16" Value="90"/>
	<EmissionPathType Type="game@GameData.Skill.EEmissionPathType" Value="Line"/>
	<FireSocketName Type="mscorlib.dll@System.String" Value=""/>
	<ThrowOffset Type="mscorlib.dll@System.Single" Value="0.5"/>
	<LevelOffset Type="mscorlib.dll@System.Single" Value="0"/>
	<ThrowRoleDistance Type="mscorlib.dll@System.Single" Value="8"/>
	<DeathTime Type="mscorlib.dll@System.Single" Value="0"/>
	<DeathOnDamage Type="mscorlib.dll@System.Boolean" Value="True"/>
	<DamageDelayTime Type="mscorlib.dll@System.Int64" Value="0"/>
	<DamageClosedRoleInterval Type="mscorlib.dll@System.Int64" Value="50"/>
	<NotifyDamageRepeat Type="mscorlib.dll@System.Boolean" Value="False"/>
	<SameRoleDamageCount Type="mscorlib.dll@System.Int32" Value="1"/>
	<DamageCalculationCount Type="mscorlib.dll@System.Int32" Value="1"/>
	<MaxDamageTargetNumber Type="mscorlib.dll@System.Int32" Value="1"/>
	<SummonRoleHasHatred Type="mscorlib.dll@System.Boolean" Value="False"/>
	<SecondFireRemoveSum Type="mscorlib.dll@System.Boolean" Value="True"/>
	<Immediate2Death Type="mscorlib.dll@System.Boolean" Value="True"/>
	<IsUpdatePos2Client Type="mscorlib.dll@System.Boolean" Value="False"/>
	<IsCollideWithScene Type="mscorlib.dll@System.Boolean" Value="True"/>
	<LenghtLocX Type="mscorlib.dll@System.Single" Value="1"/>
	<LenghtLocY Type="mscorlib.dll@System.Single" Value="1"/>
	<LenghtLocZ Type="mscorlib.dll@System.Single" Value="1"/>
	<Gravity Type="mscorlib.dll@System.Boolean" Value="False"/>
	<AIRotationAngle Type="mscorlib.dll@System.Single" Value="0"/>
	<CanFire Type="mscorlib.dll@System.Boolean" Value="False"/>
</GameData.Skill.SkillTemplate>

