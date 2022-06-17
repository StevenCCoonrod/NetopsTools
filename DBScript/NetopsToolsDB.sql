IF EXISTS (SELECT 1 FROM master.dbo.sysdatabases WHERE name = 'NetopsToolsDB')
	BEGIN
		DROP DATABASE [NetopsToolsDB]
		print '' print '> Dropping NetopsToolsDB'
	END
GO
print '' print '> Creating NetopsToolsDB'
GO
CREATE DATABASE [NetopsToolsDB]
GO
print '' print '> Using NetopsToolsDB'
GO
USE [NetopsToolsDB]
GO

CREATE TABLE [dbo].[MtrHop]
(
	[MtrHopID]			INT				NOT NULL	IDENTITY(1000000000,1),
	[HostName]			NVARCHAR(200) 	NOT NULL,
	[HopNumber]			TINYINT			NOT NULL,
	[PacketLoss]		DECIMAL			NOT NULL,
	[PacketsSent]		TINYINT			NOT NULL,
	[LastPingMS]		DECIMAL			NOT NULL,
	[AvgPingMS]			DECIMAL			NOT NULL,
	[BestPingMS]		DECIMAL			NOT NULL,
	[WorstPingMS]		DECIMAL			NOT NULL,
	[StandardDev]		DECIMAL			NOT NULL,	

	CONSTRAINT [pk_MtrHopID] PRIMARY KEY ([MtrHopID]ASC)
);
GO

CREATE TABLE [dbo].[MtrReport]
(
	[MtrReportID]		INT 			NOT NULL	IDENTITY(10000000,1),
	[SyncboxID]			NVARCHAR(12)	NOT NULL,
	[StartTime]			DATETIME		NOT NULL,
	
	CONSTRAINT [pk_MtrReportID] PRIMARY KEY ([MtrReportID]ASC)
);
GO

CREATE TABLE [dbo].[MtrReportHops]
(
	[MtrReportID]		INT				NOT NULL,
	[MtrHopID]			INT				NOT NULL,
	
	CONSTRAINT [pk_mtrReport_Hops] PRIMARY KEY ([MtrReportID]ASC,[MtrHopID]ASC),
 CONSTRAINT [fk_mtrReport] FOREIGN KEY ([MtrReportID])  REFERENCES [dbo].[MtrReport]([MtrReportID]),
 CONSTRAINT [fk_mtrHop] FOREIGN KEY ([MtrHopID])  REFERENCES [dbo].[MtrHop]([MtrHopID])
);
GO


--===============STORED PROCEDURES=================--

--INSERT Statements for SSH MTR Data--

CREATE PROCEDURE [sp_InsertMtrReport]
(
	@MtrReportID		[INT] OUTPUT,
	@SyncboxID			[NVARCHAR](12),
	@StartTime			[DATETIME]
)
AS
BEGIN
	INSERT INTO [dbo].[MtrReport]
		([SyncboxID],[StartTime])
	VALUES
		(UPPER(@SyncboxID), @StartTime)
	SET @MtrReportID = SCOPE_IDENTITY()
	SELECT @@IDENTITY
END
GO

CREATE PROCEDURE [sp_InsertMtrHop]
(
	@MtrHopID			[INT] OUTPUT,
	@HostName			NVARCHAR(200),
	@HopNumber			TINYINT,
	@PacketLoss			DECIMAL,
	@PacketsSent		TINYINT,
	@LastPingMS			DECIMAL,
	@AvgPingMS			DECIMAL,
	@BestPingMS			DECIMAL,
	@WorstPingMS		DECIMAL,
	@StandardDev		DECIMAL
)
AS
BEGIN
	INSERT INTO [dbo].[MtrHop]
		([HostName],[HopNumber],[PacketLoss],[PacketsSent],[LastPingMS],[AvgPingMS],[BestPingMS],[WorstPingMS],[StandardDev])
	VALUES
		(@HostName, @HopNumber, @PacketLoss, @PacketsSent, @LastPingMS, @AvgPingMS, @BestPingMS, @WorstPingMS, @StandardDev)
	SET @MtrHopID = SCOPE_IDENTITY()
	SELECT @@IDENTITY
END
GO

CREATE PROCEDURE [sp_InsertMtrReportHops]
(
	@MtrReportID		[INT],
	@MtrHopID			[INT]
)
AS
BEGIN
	INSERT INTO [dbo].[MtrReportHops]
		([MtrReportID],[MtrHopID])
	VALUES
		(@MtrReportID, @MtrHopID)
	RETURN @@ROWCOUNT
END
GO

--SELECT Statements

CREATE PROCEDURE [sp_SelectAllMtrs]
AS
BEGIN
	SELECT 	[dbo].[MtrReport].[MtrReportID],
			[SyncboxID],
			[StartTime],
			[dbo].[MtrHop].[MtrHopID],
			[HopNumber],
			[HostName],
			[PacketLoss],
			[PacketsSent],
			[LastPingMS],
			[AvgPingMS],
			[BestPingMS],
			[WorstPingMS],
			[StandardDev]
	FROM	[dbo].[MtrHop]
			INNER JOIN [dbo].[MtrReportHops]
		ON	[dbo].[MtrHop].[MtrHopID] = [dbo].[MtrReportHops].[MtrHopID]
			INNER JOIN [dbo].[MtrReport]
		ON	[dbo].[MtrReport].[MtrReportID] = [dbo].[MtrReportHops].[MtrReportID]
	ORDER BY [MtrReportID], [MtrHopID]
END
GO


CREATE PROCEDURE [sp_SelectAllMtrsWithinRange]
(
	@StartDatetime		[DATETIME],
	@EndDatetime		[DATETIME]
)
AS
BEGIN
	SELECT 	[dbo].[MtrReport].[MtrReportID],
			[SyncboxID],
			[StartTime],
			[dbo].[MtrHop].[MtrHopID],
			[HopNumber],
			[HostName],
			[PacketLoss],
			[PacketsSent],
			[LastPingMS],
			[AvgPingMS],
			[BestPingMS],
			[WorstPingMS],
			[StandardDev]
	FROM	[dbo].[MtrHop]
			INNER JOIN [dbo].[MtrReportHops]
		ON	[dbo].[MtrHop].[MtrHopID] = [dbo].[MtrReportHops].[MtrHopID]
			INNER JOIN [dbo].[MtrReport]
		ON	[dbo].[MtrReport].[MtrReportID] = [dbo].[MtrReportHops].[MtrReportID]
	WHERE 	[MtrReport].[StartTime] >= @StartDatetime
	AND		[MtrReport].[StartTime] <= @EndDatetime
	ORDER BY [MtrReportID], [MtrHopID]
END
GO

CREATE PROCEDURE [sp_SelectSyncboxMtrsWithinRange]
(
	@SyncboxID			[NVARCHAR](12),
	@StartDatetime		[DATETIME],
	@EndDatetime		[DATETIME]
)
AS
BEGIN
	SELECT 	[dbo].[MtrReport].[MtrReportID],
			[SyncboxID],
			[StartTime],
			[dbo].[MtrHop].[MtrHopID],
			[HopNumber],
			[HostName],
			[PacketLoss],
			[PacketsSent],
			[LastPingMS],
			[AvgPingMS],
			[BestPingMS],
			[WorstPingMS],
			[StandardDev]
	FROM	[dbo].[MtrHop]
			INNER JOIN [dbo].[MtrReportHops]
		ON	[dbo].[MtrHop].[MtrHopID] = [dbo].[MtrReportHops].[MtrHopID]
			INNER JOIN [dbo].[MtrReport]
		ON	[dbo].[MtrReport].[MtrReportID] = [dbo].[MtrReportHops].[MtrReportID]
	WHERE 	[SyncboxID] = @SyncboxID
	AND		[MtrReport].[StartTime] >= @StartDatetime
	AND		[MtrReport].[StartTime] <= @EndDatetime
	ORDER BY [MtrReportID], [MtrHopID]
END
GO

