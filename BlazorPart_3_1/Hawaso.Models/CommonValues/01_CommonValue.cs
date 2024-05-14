﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hawaso.Models.CommonValues
{
    /// <summary>
    /// [1] Model Class: CommonValue 모델 클래스 == CommonValues 테이블과 일대일로 매핑
    /// CommonValue, CommonValueModel, CommonValueViewModel, CommonValueBase, CommonValueDto, CommonValueEntity, CommonValueObject, ...
    /// </summary>
    [Table("CommonValues")]
    public class CommonValue
    {
        /// <summary>
        /// Serial Number
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 외래키
        /// </summary>
        public int? ParentId { get; set; }

        /// <summary>
        /// 외래키
        /// </summary>
        public string ParentKey { get; set; }

        /// <summary>
        /// 이름
        /// </summary>
        [Required(ErrorMessage = "이름을 입력하세요.")]
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// 제목
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 내용
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 상단 고정: 공지글로 올리기
        /// </summary>
        public bool? IsPinned { get; set; } = false;

        /// <summary>
        /// 등록자: CreatedBy, Creator
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// 등록일: Created
        /// </summary>
        //public DateTimeOffset Created { get; set; }
        public DateTime? Created { get; set; }

        /// <summary>
        /// 수정자: LastModifiedBy, ModifiedBy
        /// </summary>
        public string ModifiedBy { get; set; }

        /// <summary>
        /// 수정일: LastModified, Modified
        /// </summary>
        public DateTime? Modified { get; set; }

        #region [2] 자료실 게시판 관련 주요 컬럼
        /// <summary>
        /// 파일명
        /// </summary>
        [Display(Name = "파일")]
        public string FileName { get; set; }

        /// <summary>
        /// 파일크기
        /// </summary>
        [Display(Name = "파일크기")]
        public int FileSize { get; set; }

        /// <summary>
        /// 다운수 
        /// </summary>
        [Display(Name = "다운수")]
        public int DownCount { get; set; }
        #endregion

        #region 답변형 게시판 관련 주요 속성
        /// <summary>
        /// 참조(부모글)
        /// </summary>
        public int? Ref { get; set; }

        /// <summary>
        /// 답변깊이(레벨)
        /// </summary>
        public int? Step { get; set; }

        /// <summary>
        /// 답변순서
        /// </summary>
        public int? RefOrder { get; set; }
        #endregion

        #region 공통코드관리
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string VariableText { get; set; }
        public string VariableValue { get; set; }
        #endregion
    }
}
