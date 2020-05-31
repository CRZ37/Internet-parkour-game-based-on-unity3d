/*
 Navicat Premium Data Transfer

 Source Server         : fucql
 Source Server Type    : MySQL
 Source Server Version : 50720
 Source Host           : localhost:3306
 Source Schema         : parkour

 Target Server Type    : MySQL
 Target Server Version : 50720
 File Encoding         : 65001

 Date: 31/05/2020 09:56:38
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for coin
-- ----------------------------
DROP TABLE IF EXISTS `coin`;
CREATE TABLE `coin`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userid` int(11) NULL DEFAULT NULL,
  `coinnum` int(255) NULL DEFAULT 0,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of coin
-- ----------------------------
INSERT INTO `coin` VALUES (1, 1, 10000);
INSERT INTO `coin` VALUES (2, 2, 9629);
INSERT INTO `coin` VALUES (3, 3, 664);
INSERT INTO `coin` VALUES (4, 4, 7200);
INSERT INTO `coin` VALUES (5, 5, 9500);
INSERT INTO `coin` VALUES (6, 7, 400);
INSERT INTO `coin` VALUES (7, 8, 6);

-- ----------------------------
-- Table structure for itemprice
-- ----------------------------
DROP TABLE IF EXISTS `itemprice`;
CREATE TABLE `itemprice`  (
  `id` int(5) NOT NULL AUTO_INCREMENT,
  `shopid` int(10) NULL DEFAULT NULL,
  `itemid` int(10) NULL DEFAULT NULL,
  `price` int(10) NULL DEFAULT NULL,
  `itemname` varchar(40) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of itemprice
-- ----------------------------
INSERT INTO `itemprice` VALUES (1, 1, 1, 400, 'health');
INSERT INTO `itemprice` VALUES (2, 1, 2, 700, 'bighealth');
INSERT INTO `itemprice` VALUES (3, 1, 3, 500, 'skilltime');
INSERT INTO `itemprice` VALUES (4, 1, 4, 900, 'bigskilltime');
INSERT INTO `itemprice` VALUES (5, 2, 1, 0, 'rolemaleprice');
INSERT INTO `itemprice` VALUES (6, 2, 2, 2000, 'rolecopprice');
INSERT INTO `itemprice` VALUES (7, 2, 3, 3000, 'rolerobotprice');

-- ----------------------------
-- Table structure for playerstate
-- ----------------------------
DROP TABLE IF EXISTS `playerstate`;
CREATE TABLE `playerstate`  (
  `id` int(11) NOT NULL AUTO_INCREMENT COMMENT '玩家状态id',
  `userid` int(11) NULL DEFAULT NULL COMMENT '用户id',
  `health` int(11) NULL DEFAULT 6 COMMENT '玩家生命值',
  `skilltime` float(11, 1) NULL DEFAULT 2.0 COMMENT '玩家技能时间',
  `roleselect` varchar(11) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL COMMENT '玩家选择的角色',
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of playerstate
-- ----------------------------
INSERT INTO `playerstate` VALUES (3, 2, 23, 4.0, '3');
INSERT INTO `playerstate` VALUES (5, 4, 21, 2.0, '1');
INSERT INTO `playerstate` VALUES (6, 5, 6, 2.5, '1');
INSERT INTO `playerstate` VALUES (7, 3, 17, 2.0, '1');

-- ----------------------------
-- Table structure for result
-- ----------------------------
DROP TABLE IF EXISTS `result`;
CREATE TABLE `result`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userid` int(11) NOT NULL,
  `totalcount` int(255) NOT NULL DEFAULT 0,
  `wincount` int(255) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fk_userid`(`userid`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 11 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of result
-- ----------------------------
INSERT INTO `result` VALUES (5, 3, 24, 14);
INSERT INTO `result` VALUES (6, 2, 32, 14);
INSERT INTO `result` VALUES (7, 5, 1, 1);
INSERT INTO `result` VALUES (8, 1, 6, 3);
INSERT INTO `result` VALUES (9, 7, 1, 1);
INSERT INTO `result` VALUES (10, 8, 1, 0);

-- ----------------------------
-- Table structure for roleshopstate
-- ----------------------------
DROP TABLE IF EXISTS `roleshopstate`;
CREATE TABLE `roleshopstate`  (
  `id` int(5) NOT NULL AUTO_INCREMENT,
  `userid` int(5) NULL DEFAULT NULL,
  `rolebuy` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of roleshopstate
-- ----------------------------
INSERT INTO `roleshopstate` VALUES (1, 2, '1,1,1');
INSERT INTO `roleshopstate` VALUES (3, 2, '1,1,1');

-- ----------------------------
-- Table structure for shopstate
-- ----------------------------
DROP TABLE IF EXISTS `shopstate`;
CREATE TABLE `shopstate`  (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `userid` int(11) NULL DEFAULT NULL,
  `healthtime` int(11) NULL DEFAULT NULL,
  `bighealthtime` int(11) NULL DEFAULT NULL,
  `skilltimetime` int(11) NULL DEFAULT NULL,
  `bigskilltimetime` int(11) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 6 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of shopstate
-- ----------------------------
INSERT INTO `shopstate` VALUES (2, 2, 1, 1, 2, 1);
INSERT INTO `shopstate` VALUES (3, 4, 2, 0, 0, 0);
INSERT INTO `shopstate` VALUES (4, 5, 0, 0, 1, 0);
INSERT INTO `shopstate` VALUES (5, 3, 3, 4, 0, 0);

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user`  (
  `id` int(255) NOT NULL AUTO_INCREMENT COMMENT '用户id',
  `username` varchar(45) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '用户名',
  `password` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL COMMENT '用户密码',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `username`(`username`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES (1, 'ccc', 'ccc');
INSERT INTO `user` VALUES (2, 'testaccount', 'test');
INSERT INTO `user` VALUES (3, 'justtest', 'adad');
INSERT INTO `user` VALUES (4, 'eee', 'qqq');
INSERT INTO `user` VALUES (5, 'qwee', 'qwee');
INSERT INTO `user` VALUES (7, 'chenrunze', 'qwer');
INSERT INTO `user` VALUES (8, 'crztest', 'crz');
INSERT INTO `user` VALUES (9, 'ceshi', 'ceshi');

SET FOREIGN_KEY_CHECKS = 1;
