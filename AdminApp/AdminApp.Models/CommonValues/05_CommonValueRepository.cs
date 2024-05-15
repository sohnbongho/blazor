﻿using Dul.Articles;
using Dul.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hawaso.Models.CommonValues
{
    /// <summary>
    /// [5] Repository Class: ADO.NET or Dapper or Entity Framework Core
    /// </summary>
    public class CommonValueRepository : ICommonValueRepository
    {
        private readonly CommonValueDbContext _context;
        private readonly ILogger _logger;

        public CommonValueRepository(CommonValueDbContext context, ILoggerFactory loggerFactory)
        {
            this._context = context;
            this._logger = loggerFactory.CreateLogger(nameof(CommonValueRepository));
        }

        //[6][1] 입력
        public async Task<CommonValue> AddAsync(CommonValue model)
        {
            #region CommonValue 기능 추가
            // 현재테이블의 Ref의 Max값 가져오기
            int maxRef = 1;
            int? max = _context.CommonValues.Max(m => m.Ref);
            if (max.HasValue)
            {
                maxRef = (int)max + 1;
            }

            model.Ref = maxRef;
            model.Step = 0;
            model.RefOrder = 0;
            #endregion

            try
            {
                _context.CommonValues.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
            }

            return model;
        }

        //[6][2] 출력
        public async Task<List<CommonValue>> GetAllAsync()
        {
            return await _context.CommonValues.OrderByDescending(m => m.Id)
                //.Include(m => m.CommonValuesComments)
                .ToListAsync();
        }

        //[6][3] 상세
        public async Task<CommonValue> GetByIdAsync(int id)
        {
            return await _context.CommonValues
                //.Include(m => m.CommonValuesComments)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        //[6][4] 수정
        public async Task<bool> EditAsync(CommonValue model)
        {
            try
            {
                _context.CommonValues.Attach(model);
                _context.Entry(model).State = EntityState.Modified;
                return (await _context.SaveChangesAsync() > 0 ? true : false);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(EditAsync)}): {e.Message}");
            }

            return false;
        }

        //[6][5] 삭제
        public async Task<bool> DeleteAsync(int id)
        {
            //var model = await _context.CommonValues.SingleOrDefaultAsync(m => m.Id == id);
            try
            {
                var model = await _context.CommonValues.FindAsync(id);
                _context.Remove(model);
                return (await _context.SaveChangesAsync() > 0 ? true : false);
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(DeleteAsync)}): {e.Message}");
            }

            return false;
        }

        //[6][6] 페이징
        public async Task<PagingResult<CommonValue>> GetAllAsync(int pageIndex, int pageSize)
        {
            var totalRecords = await _context.CommonValues.CountAsync();
            var models = await _context.CommonValues
                .OrderByDescending(m => m.Id)
                //.Include(m => m.CommonValuesComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<CommonValue>(models, totalRecords);
        }

        // 부모
        public async Task<PagingResult<CommonValue>> GetAllByParentIdAsync(int pageIndex, int pageSize, int parentId)
        {
            var totalRecords = await _context.CommonValues.Where(m => m.ParentId == parentId).CountAsync();
            var models = await _context.CommonValues
                .Where(m => m.ParentId == parentId)
                .OrderByDescending(m => m.Id)
                //.Include(m => m.CommonValuesComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<CommonValue>(models, totalRecords);
        }

        // 상태
        public async Task<Tuple<int, int>> GetStatus(int parentId)
        {
            var totalRecords = await _context.CommonValues.Where(m => m.ParentId == parentId).CountAsync();
            var pinnedRecords = await _context.CommonValues.Where(m => m.ParentId == parentId && m.IsPinned == true).CountAsync();

            return new Tuple<int, int>(pinnedRecords, totalRecords); // (2, 10)
        }

        // DeleteAllByParentId
        public async Task<bool> DeleteAllByParentId(int parentId)
        {
            try
            {
                var models = await _context.CommonValues.Where(m => m.ParentId == parentId).ToListAsync();

                foreach (var model in models)
                {
                    _context.CommonValues.Remove(model);
                }

                return (await _context.SaveChangesAsync() > 0 ? true : false);

            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(DeleteAllByParentId)}): {e.Message}");
            }

            return false;
        }

        // 검색
        public async Task<PagingResult<CommonValue>> SearchAllAsync(int pageIndex, int pageSize, string searchQuery)
        {
            var totalRecords = await _context.CommonValues
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .CountAsync();
            var models = await _context.CommonValues
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .OrderByDescending(m => m.Id)
                //.Include(m => m.CommonValuesComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<CommonValue>(models, totalRecords);
        }

        public async Task<PagingResult<CommonValue>> SearchAllByParentIdAsync(int pageIndex, int pageSize, string searchQuery, int parentId)
        {
            var totalRecords = await _context.CommonValues.Where(m => m.ParentId == parentId)
                .Where(m => EF.Functions.Like(m.Name, $"%{searchQuery}%") || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .CountAsync();
            var models = await _context.CommonValues.Where(m => m.ParentId == parentId)
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .OrderByDescending(m => m.Id)
                //.Include(m => m.CommonValuesComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<CommonValue>(models, totalRecords);
        }

        public async Task<SortedList<int, double>> GetMonthlyCreateCountAsync()
        {
            SortedList<int, double> createCounts = new SortedList<int, double>();

            // 1월부터 12월까지 0.0으로 초기화
            for (int i = 1; i <= 12; i++)
            {
                createCounts[i] = 0.0;
            }

            for (int i = 0; i < 12; i++)
            {
                // 현재 달부터 12개월 전까지 반복
                var current = DateTime.Now.AddMonths(-i);
                var cnt = _context.CommonValues.AsEnumerable().Where(
                    m => m.Created != null
                    &&
                    Convert.ToDateTime(m.Created).Month == current.Month
                    &&
                    Convert.ToDateTime(m.Created).Year == current.Year
                ).ToList().Count();
                createCounts[current.Month] = cnt;
            }

            return await Task.FromResult(createCounts);
        }

        public async Task<PagingResult<CommonValue>> GetAllByParentKeyAsync(int pageIndex, int pageSize, string parentKey)
        {
            var totalRecords = await _context.CommonValues.Where(m => m.ParentKey == parentKey).CountAsync();
            var models = await _context.CommonValues
                .Where(m => m.ParentKey == parentKey)
                .OrderByDescending(m => m.Id)
                //.Include(m => m.CommonValuesComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<CommonValue>(models, totalRecords);
        }

        public async Task<PagingResult<CommonValue>> SearchAllByParentKeyAsync(int pageIndex, int pageSize, string searchQuery, string parentKey)
        {
            var totalRecords = await _context.CommonValues.Where(m => m.ParentKey == parentKey)
                .Where(m => EF.Functions.Like(m.Name, $"%{searchQuery}%") || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .CountAsync();
            var models = await _context.CommonValues.Where(m => m.ParentKey == parentKey)
                .Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.Title.Contains(searchQuery))
                .OrderByDescending(m => m.Id)
                //.Include(m => m.CommonValuesComments)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagingResult<CommonValue>(models, totalRecords);
        }

        public async Task<ArticleSet<CommonValue, int>> GetArticles<TParentIdentifier>(int pageIndex, int pageSize, string searchField, string searchQuery, string sortOrder, TParentIdentifier parentIdentifier)
        {
            // Default Values
            if (!_context.CommonValues.Any())
            {
                var model = new CommonValue() { Name = " ", Category = "Projects", SubCategory = "Vetting Steps", VariableText = "# of Signers", VariableValue = "2" };
                await AddAsync(model);

                _context.SaveChanges();
            }

            //var items = from m in _context.CommonValues select m; // 쿼리 구문(Query Syntax)
            var items = _context.CommonValues.Select(m => m); // 메서드 구문(Method Syntax)

            // ParentBy 
            if (parentIdentifier is int parentId && parentId != 0)
            {
                items = items.Where(m => m.ParentId == parentId);
            }
            else if (parentIdentifier is string parentKey && !string.IsNullOrEmpty(parentKey))
            {
                items = items.Where(m => m.ParentKey == parentKey);
            }

            // Search Mode
            if (!string.IsNullOrEmpty(searchQuery))
            {
                if (searchField == "Name")
                {
                    // Name
                    items = items.Where(m => m.Name.Contains(searchQuery));
                }
                else if (searchField == "Title")
                {
                    // Title
                    items = items.Where(m => m.Title.Contains(searchQuery));
                }
                else
                {
                    // All
                    items = items.Where(m => m.Name.Contains(searchQuery) || m.Title.Contains(searchQuery) || m.VariableText.Contains(searchQuery) || m.VariableValue.Contains(searchQuery) || m.Category.Contains(searchQuery) || m.SubCategory.Contains(searchQuery));
                }
            }

            var totalCount = await items.CountAsync();

            // Sorting
            switch (sortOrder)
            {
                case "Name":
                    items = items.OrderBy(m => m.Name);
                    break;
                case "NameDesc":
                    items = items.OrderByDescending(m => m.Name);
                    break;
                case "Title":
                    items = items.OrderBy(m => m.Title);
                    break;
                case "TitleDesc":
                    items = items.OrderByDescending(m => m.Title);
                    break;
                case "Category":
                    items = items.OrderBy(m => m.Category);
                    break;
                case "CategoryDesc":
                    items = items.OrderByDescending(m => m.Category);
                    break;
                case "Sub":
                    items = items.OrderBy(m => m.SubCategory);
                    break;
                case "SubDesc":
                    items = items.OrderByDescending(m => m.SubCategory);
                    break;
                case "VariableText":
                    items = items.OrderBy(m => m.VariableText);
                    break;
                case "VariableTextDesc":
                    items = items.OrderByDescending(m => m.VariableText);
                    break;
                case "VariableValue":
                    items = items.OrderBy(m => m.VariableValue);
                    break;
                case "VariableValueDesc":
                    items = items.OrderByDescending(m => m.VariableValue);
                    break;
                default:
                    items = items.OrderByDescending(m => m.Ref).ThenBy(m => m.RefOrder);
                    break;
            }

            // Paging
            items = items.Skip(pageIndex * pageSize).Take(pageSize);

            return new ArticleSet<CommonValue, int>(await items.ToListAsync(), totalCount);
        }

        // 답변
        public async Task<CommonValue> AddAsync(CommonValue model, int parentRef, int parentStep, int parentOrder)
        {
            // 비집고 들어갈 자리 
            var replys = await _context.CommonValues.Where(m => m.Ref == parentRef && m.RefOrder > parentOrder).ToListAsync();
            foreach (var item in replys)
            {
                item.RefOrder = item.RefOrder + 1;
                try
                {
                    _context.CommonValues.Attach(item);
                    _context.Entry(item).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
                }
            }

            model.Step = parentStep + 1;
            model.RefOrder = parentOrder + 1;

            try
            {
                _context.CommonValues.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError($"ERROR({nameof(AddAsync)}): {e.Message}");
            }

            return model;
        }

        public async Task<string> FindValueByTest(string text)
        {
            var result = await _context.CommonValues.SingleOrDefaultAsync(m => m.VariableText == text);
            if (result != null)
            {
                return result.VariableValue;
            }
            else
            {
                return "";
            }
        }
    }
}
